using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public CameraManager cameraManager;
    public Player player;
    public UiController uiController;
    public TutorialTrigger[] tutorialTrigger;

    public bool isPauseTutorial; // 튜토리얼 일시정지 체크
    public bool isGameOver = false;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
       
        //씬이 변경되도 파괴하지 않는 기능 
        GameObject.DontDestroyOnLoad(this.gameObject);
    }

    void Update()
    {
        // GameOver();
    }


}
