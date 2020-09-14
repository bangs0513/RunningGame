using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public CameraManager cameraManager;

    int a;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
       
        //씬이 변경되도 파괴하지 않는 기능 
        GameObject.DontDestroyOnLoad(this.gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
