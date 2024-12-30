using System.Collections;
using TMPro;
using UnityEngine;

public class Timer : MonoBehaviour
{
    public TextMeshProUGUI PlayTimerText;
    public TextMeshProUGUI GameOverCountScoreText;//개의 눈사람을 완성했어
    public TextMeshProUGUI GameOverTimerScoreText;//눈사람 만든 시간은
    
    public float TimeRemaining = 30.0f;  // 게임 가능/남은 시간 -> 처음 30초로 시작
    private int sum = 0;//만든 눈사람의 개수
    private float timeTotal = 0.0f;// 총 플레이한 시간
    //private int SnowManCount = 0;//눈사람 만들어질때마다 MakeSnow 에서
    
    public GameObject SnowMachine;
    public GameObject GameOverUI;
    public GameObject PlayUI;
    public GameObject IntroUI;
    
    
    
    
    public void AddTime(float point)//눈사람 만들어서 10초 추가
    {
        if (TimeRemaining > 0.0f)  // 게임 시간이 남았다면
        {
            TimeRemaining += point;
            UpdateTimerUI();
        }
    }

    void Update()
    {
        timeTotal += Time.deltaTime;
        if (TimeRemaining > 0.0f)
        {
            TimeRemaining -= Time.deltaTime;
            UpdateTimerUI();
        }
        else
        {
            TimeRemaining = 0.0f;
            SnowMachine.SetActive(false);
            GameOverUI.SetActive(true);
            PlayUI.SetActive(false);
            
            //모든 오브젝트 삭제
            RemoveSnowObjects();
            
            GameOverCountScoreText.text = sum.ToString() + "개의 눈사람을 완성했어!";
            GameOverTimerScoreText.text = "눈사람을 만든 시간: "+timeTotal.ToString("0.00");
            sum = 0;
            timeTotal = 0.0f;
        }
    }

    void UpdateTimerUI()
    {
        if (PlayTimerText != null)
        {
            PlayTimerText.text = TimeRemaining.ToString("0.00"); 
        }
        else
        {
            Debug.Log($"null");  
        }
    }
    
    
    void RemoveSnowObjects()
    {
        // SnowMachine 아래에 있는 'Snow' 태그를 가진 모든 오브젝트 찾기
        GameObject[] snowObjects = GameObject.FindGameObjectsWithTag("Snow");//눈덩이Snow객체
        GameObject[] dotObjects = GameObject.FindGameObjectsWithTag("Dot");//궤적Dot객체
        
        foreach (GameObject snow in snowObjects)
        {
            Destroy(snow);
        }
        
        foreach (GameObject dot in dotObjects)
        {
            Destroy(dot);
        }

        TimeRemaining = 30.0f;
    }


    public void SnowManCountScore(int SnowManCount)
    {
        sum += SnowManCount;
    }

    public void RetryButton()
    {
        TimeRemaining = 0.0f;
        SnowMachine.SetActive(true);
        GameOverUI.SetActive(false);
        PlayUI.SetActive(true);
        
        RemoveSnowObjects();
        GameOverCountScoreText.text = sum.ToString()+"명의 눈사람을 완성했어!";
        sum = 0;
    }

    public void IntroButton()
    {
        TimeRemaining = 0.0f;
        SnowMachine.SetActive(false);
        GameOverUI.SetActive(false);
        PlayUI.SetActive(false);
        IntroUI.SetActive(true);
        
        RemoveSnowObjects();
        GameOverCountScoreText.text = sum.ToString()+"개의 눈사람을 완성했어!";
        sum = 0;
    }
}