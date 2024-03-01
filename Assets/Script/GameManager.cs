using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditorInternal.Profiling.Memory.Experimental;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static UnityEditor.Experimental.GraphView.GraphView;

public class GameManager : MonoBehaviour
{
    public Camera mainCamera;
    public bool IsPause = false;
    public static int totalPoint;
    public int stagePoint = 0;
    public int stageIndex = 0;
    public int HP = 6;
    public TextMeshProUGUI ScoreMSG;
    public TextMeshProUGUI totalScoreMSG;
    public List<Image> Hearts = new List<Image> ();

    public PlayerMove player;

    public GameObject UIRestartBtn;
    public GameObject UIClearBtn;
    public GameObject UIClearExitBtn;
    public GameObject EndingMSG;
    public GameObject UIExitBtn;


    public void EnemyHealthDown()
    {
        if (HP > 0)
        {
            HP -= 1;
        }
        else
        {
            player.OnDie();
        }
    }

    public void DropHealthDown()
    {
        if (HP > 0)
        {
            HP -= 2;
        }
        else
        {
            player.OnDie();
        }
    }
    public void SetText()
    {
        ScoreMSG.text = "Score : " + stagePoint.ToString();
    }

    public void SetTotalScore()
    {
        totalScoreMSG.text = "Total Score : " + totalPoint.ToString();
    }

    public void HeartControl()
    {
        int heartsToDisable = 6 - HP;

        if(HP > 0)
        {
            for (int i = 0; i < heartsToDisable; i++)
            {
                Hearts[i].gameObject.SetActive(false);
            }
        }
    }

    public void Start()
    {
        if (SceneManager.GetActiveScene().buildIndex >= 2)
        {
            HP = 6;
        }
    }
    private void Update()
    {

        if (player.isclear)
        {
            player.isclear = false;
            NextStage();
        }

        if (Input.GetKeyDown(KeyCode.Escape) && IsPause == false)
        {
            Time.timeScale = 0;
            UIRestartBtn.SetActive(true);
            UIExitBtn.SetActive(true);
            IsPause = true;
        }
        else if (Input.GetKeyDown(KeyCode.Escape) && IsPause == true)
        {
            Time.timeScale = 1;
            UIRestartBtn.SetActive(false);
            UIExitBtn.SetActive(false);
            IsPause = false;
        }
    }

    public void NextStage()
    {
        stageIndex = SceneManager.GetActiveScene().buildIndex;

        if (stageIndex < SceneManager.sceneCountInBuildSettings - 1)
        {
            HP = 6;
            totalPoint += stagePoint;
            Debug.Log("합산" + totalPoint);
            stageIndex++;
            SceneManager.LoadScene(stageIndex);
        }
        else //게임 클리어
        {
            totalPoint +=stagePoint;
            Debug.Log("합산 완료 " + totalPoint);

            //시간 정지
            Time.timeScale = 0;

            //재시작 버튼 UI
            SetTotalScore();
            EndingMSG.SetActive(true);
            UIClearBtn.SetActive(true);
            UIClearExitBtn.SetActive(true);

            // 최종 스코어 초기화
            totalPoint = 0;
            Debug.Log(totalPoint);
        }
    }

    public void Restart()
    {
        Time.timeScale = 1;
        stageIndex = 0;
        SceneManager.LoadScene(stageIndex);
    }

    public void Dead()
    {
        player.OnDie();
        UIRestartBtn.SetActive(true);
        UIExitBtn.SetActive(true);
    }

    public void GameExit()
    {
        Application.Quit();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            Dead();
            mainCamera.transform.parent = null;
        }
    }
}
