using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;


public class MakeSnow : MonoBehaviour
{
    private float keepTimer = 0; //눈사람이 2초동안 유지가 됐는지 확인하는 timer
    
    private GameObject[] snowObjectArray;//만들어진 Snow객체 리스트
    private bool isBuilding = false;//눈덩이 3개 쌓기 체크
    
    public Timer TimerScript;
    public GameObject[] DecoElement;//[0]: face, [1]: 모자, [2]: 머플러 -> 눈사람 꾸미기 요소 배열로 저장, 가져와서 쓰기 편하게
    private GameObject[] DecoElementObjectArray;// DecoElement를 Instanciate한 Object의 배열
    
    void Start()
    {
        TimerScript = FindObjectOfType<Timer>();//Timer라는 스크립트 찾아 넣기(Timer안에 있는 텍스트 객체를 가져와야해서)
        
    }
    private void Update()
    {
        if (isBuilding)
        {
            keepTimer += Time.deltaTime;
            
            if (keepTimer >= 2f)
            {
                CompleteSnowman();
            }
        }
        else
        {
            keepTimer = 0;
        }
        
    }
    
    private void OnCollisionEnter2D(Collision2D collision)//눈덩이가 닿으면
    {
        
        snowObjectArray= GameObject.FindGameObjectsWithTag("Snow");//Snow태그를 가진 Snow객체 넣기
        switch (collision.gameObject.tag)
        {
            case "Ground":
                if (snowObjectArray.Length ==1)
                {
                    Debug.Log("땅에 닿았다");
                }
                else
                {
                    if (snowObjectArray[snowObjectArray.Length-1]==gameObject)
                    {
                        Destroy(gameObject);
                        Debug.Log("잘못쌓았다");
                    }
                }
                break;
            
            case "Snow":
                if (snowObjectArray.Length >1)
                {
                    Debug.Log("쌓았다");
                    if (snowObjectArray.Length == 3)//눈사람 3개 만들었다면
                    {
                        isBuilding = true; //3개 쌓기 성공
                        Deco();//꾸미기
                    }
                }
                break;
            
            
            case "Wall":
                Destroy(gameObject);
                Debug.Log("뒤로 넘어갔다");
                break;
            
        }
        
    }
    
    private void OnCollisionExit2D(Collision2D collision)//눈덩이가 떨어지면
    {
        // 눈이 떨어지면 즉시 실패 처리
        if (collision.gameObject.CompareTag("Snow"))
        {
            isBuilding = false;
            keepTimer = 0;  // 즉시 타이머 초기화
            
            if (DecoElementObjectArray != null)
            {
                DeleteDeco(DecoElementObjectArray);//떨어졌으니 Deco넣어 놓은 객체 모두 삭제
            }
            
            
        }
    }
    

    public void CompleteSnowman()
    {
        if (snowObjectArray!=null)
        {
            foreach (GameObject snowObject in snowObjectArray)
            {
                Destroy(snowObject);  // 각 Snow 오브젝트 삭제
            }
            snowObjectArray = null;
            keepTimer = 0;
            Debug.Log("눈사람 완성!");
            isBuilding = false;
        }
        if (TimerScript != null)
        {
            TimerScript.AddTime(10.0f);//만들기 성공했으니 10초 추가
            Debug.Log("타이머 10초 추가");
            TimerScript.SnowManCountScore(1);//Timer스크립트의 SnowManCountScore에 스노우맨 개수(1개) 넣어 전송
        }
    }


    public void Deco() //눈사람 만들어지면 얼굴, 모자, 머플러로 꾸미기
    {
        if (snowObjectArray == null || snowObjectArray.Length < 3 || snowObjectArray[2] == null)
        {
            return;
        }

        DecoElementObjectArray = new GameObject[snowObjectArray.Length];
        
        //[0]: 얼굴, [1]: 모자, [2]: 머플러
        //얼굴
        DecoElementObjectArray[0] = Instantiate(DecoElement[0], 
            new Vector2(snowObjectArray[2].transform.position.x, snowObjectArray[2].transform.position.y), 
        Quaternion.identity);
        
        //모자
        DecoElementObjectArray[1] = Instantiate(DecoElement[1], 
            new Vector2(snowObjectArray[2].transform.position.x, snowObjectArray[2].transform.position.y+1.8f), 
        Quaternion.identity);
        
        //머플러
        int MuffColor = Random.Range(2, 5);//머플러 색깔 랜덤하게 2: red, 3: green, 4: green
        DecoElementObjectArray[2] = Instantiate(DecoElement[MuffColor], 
            new Vector2(snowObjectArray[1].transform.position.x, snowObjectArray[1].transform.position.y+1.2f), 
            Quaternion.identity);
            
        DecoElementObjectArray = GameObject.FindGameObjectsWithTag("Deco");
    }

    public void DeleteDeco(GameObject[] decoArray)
    {
        if (decoArray == null || decoArray.Length == 0)
        {
            return;
        }

        foreach (var o in decoArray)
        {
            if (o != null)
            {
                Destroy(o);
            }
        }
        DecoElementObjectArray = null;
        
    }
    
}
