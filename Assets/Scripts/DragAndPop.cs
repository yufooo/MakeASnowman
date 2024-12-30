using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class DragAndPop : MonoBehaviour, 
    IPointerDownHandler, 
    IPointerUpHandler, 
    IDragHandler
{ 
    private Vector2 startPosition; // 객체를 드래그하기 시작한 위치
    private Vector2 pullPosition;  // 현재 마우스 포인터의 위치
    
    private Camera mainCamera; // 카메라 객체 (마우스 위치를 월드 좌표로 변환하기 위해 사용)
    private float maxPullDistance; // 드래그 가능한 최대 거리
    
    public GameObject Snow; //발사할 눈덩이
    public GameObject FirePoint; //발사 포인트
    
    public float Power = 500.0f;//궤적 힘  
    public float Mass = 5.0f;//궤적 질량
    public int MaxStep = 20;//궤적 스탭
    public float TimeStep = 0.1f;
    public GameObject Trajectory;//궤적 객체
    public List<GameObject> Objects = new List<GameObject>();//궤적 리스트
    
    private Vector2 handleButtonPosition; //HandleButton의 처음 position을 가지고 계속 그 위치에 놓기 위해
    
    private void Awake()
    {
        mainCamera = Camera.main; // MainCamera를 찾음. (기본 카메라)
        handleButtonPosition = new Vector2(transform.position.x, transform.position.y); //HandleButton position 저장
    }
    
    
    //포물선 궤적을 계산하는 함수
    private List<Vector2> PredictTrajectory(Vector2 force, float mass)
    {
        
        List<Vector2> trajectory = new List<Vector2>();//궤적을 저장할 리스트
        
        Vector2 position = FirePoint.transform.position;//발사 지점
        Vector2 velocity = force / mass;//발사 속도 f=ma
    
        trajectory.Add(position);//첫번째 위치 저장
    
        for (int i = 1; i <= MaxStep; i++) //최대 단계까지 예측을 진행
        {
            float timeElapsed = TimeStep * i; //각 단계의 시간 경과
            Vector2 gravity = new Vector2(0, Physics.gravity.y); // Vector2로
            // 등가속도 운동 공식을 이용하여 위치 계산
            trajectory.Add(position + 
                           velocity * 
                           timeElapsed + 
                           gravity * (0.5f * 
                                              timeElapsed * 
                                              timeElapsed));
            //충돌여부확인(궤적 상의 두 점 사이에)
            if (CheckCollision(trajectory[i - 1], trajectory[i], out Vector2 hitPoint))
            {
                trajectory[i] = hitPoint;//충돌지점으로 궤적을 수정
                break;// 충돌 후 예측을 중단
            }
        }
    
        return trajectory; //계산된 궤적 반환
    }
    
    //충돌을 검사하는 함수(두 점 사이에서 Raycast를 사용)
    private bool CheckCollision(Vector2 start, Vector2 end, out Vector2 hitPoint)
    {
        hitPoint = end;//기본적으로 충돌 점을 끝 점으로 설정
        Vector2 direction = end - start; //두 점 사이의 방향 벡터
        float distance = direction.magnitude; //방향 벡터의 크기
        
        //Raycast로 충돌 여부를 확인(충돌 레이어는 'Default')
        if (Physics2D.Raycast(start, direction.normalized, distance, 1 << LayerMask.NameToLayer("Default")))
        {
            hitPoint = start+direction.normalized*distance; //충돌 지점을 반환
            return true;//충돌이 발생했음을 반환
        }
        
        return false; //충돌이 없으면 false 반환
    }
    

    // 마우스 버튼을 눌렀을 때 호출되는 메서드
    public void OnPointerDown(PointerEventData eventData)
    {
        if (Snow != null)
        {
            
            // 마우스 클릭 시, 해당 위치를 월드 좌표로 변환하여 저장
            pullPosition = startPosition = mainCamera.ScreenToWorldPoint(new Vector2(eventData.position.x, eventData.position.y));
            
        
            foreach (var o in Objects)
            {
                if (o != null)
                {
                    Destroy(o);
                }
                
            }
            
         
            List<Vector2> trajectorys = PredictTrajectory(Snow.transform.right * Power, Mass);
            //새로운 궤적을 표시할 오브젝트 생성
            foreach (var trajectory in trajectorys)
            {
                var go = Instantiate(Trajectory, trajectory, Quaternion.identity);//궤적 위치에 오브젝트 생성
                Objects.Add(go);
            }
            if (Objects.Count == trajectorys.Count)
            {
                for (var index = 0; index < trajectorys.Count; index++)
                {
                    var trajectory = trajectorys[index];
                    Objects[index].SetActive(true);
                    Objects[index].transform.position = trajectory;
                }
            }
            
        }
        
    }
    
    
     // 드래그 중 계속 호출되는 메서드
    public void OnDrag(PointerEventData eventData)
    {
        
        if (Camera.main != null) // 카메라가 존재하면 (null 체크)
        {
            
                 // 마우스 위치를 월드 좌표로 변환
            Vector2 mouseWorldPos = mainCamera.ScreenToWorldPoint(
                new Vector2(eventData.position.x, 
                eventData.position.y));
            // 드래그한 위치와 시작 위치 간의 벡터 계산
            Vector2 pullDirection = startPosition - mouseWorldPos;
            
            //FirePoint 방향 움직임
            float angle = Mathf.Atan2(pullDirection.y, pullDirection.x) * Mathf.Rad2Deg;
            FirePoint.transform.rotation = Quaternion.Euler(0, 0, angle+90f);
            
            // 드래그가 maxPullDistance를 넘으면, 그 최대값으로 제한
            if (pullDirection.magnitude > maxPullDistance)
            {
                pullDirection = pullDirection.normalized * maxPullDistance; // 방향 벡터의 크기를 maxPullDistance로 제한
               
            }
            
            // 객체의 위치를 마우스 위치로 업데이트
            transform.position = mouseWorldPos;

            foreach (var o in Objects)
            {
                if (o != null)
                {
                    Destroy(o);
                }
                
            }
            
            Objects.Clear();
            
            List<Vector2> trajectorys = PredictTrajectory(FirePoint.transform.up * Power, Mass);
            //새로운 궤적을 표시할 오브젝트 생성
            foreach (var trajectory in trajectorys)
            {
                var go = Instantiate(Trajectory, trajectory, Quaternion.identity);//궤적 위치에 오브젝트 생성
                Objects.Add(go);
            }
            
            if (Objects.Count == trajectorys.Count)
            {
                for (var index = 0; index < trajectorys.Count; index++)
                {
                    var trajectory = trajectorys[index];
                    Objects[index].SetActive(true);
                    Objects[index].transform.position = trajectory;
                }
            }
            
        }
        
    }
    
    // 마우스 버튼을 떼었을 때 호출되는 메서드
    public void OnPointerUp(PointerEventData eventData)
    {
        
        GameObject instantiatedSnow = Instantiate(Snow, FirePoint.transform.position, Snow.transform.rotation);//눈덩이 생성
        
        // 눈덩이 Animator 실행
        Animator snowAnimator = instantiatedSnow.GetComponent<Animator>();
    
        if (snowAnimator != null)
        {
            snowAnimator.SetTrigger("Fire");
        }
        else
        {
            Debug.LogWarning("Snow 프리팹에 Animator가 없습니다.");
        }
        
        instantiatedSnow.GetComponent<Rigidbody2D>().mass = Mass;//눈덩이에 질량처리
        instantiatedSnow.GetComponent<Rigidbody2D>().AddForce(FirePoint.transform.up * Power, ForceMode2D.Impulse);//눈덩이 발사
        
        
        foreach (var o in Objects)
        {
            if (o != null)
            {
                Destroy(o);
            }
                
        }
        
        Objects.Clear(); 
        
        transform.position = new Vector2(handleButtonPosition.x, handleButtonPosition.y);
        FirePoint.transform.rotation = Quaternion.Euler(0,0,-45f);
        
        
    }
    
    

    
        

   
}
