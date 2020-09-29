using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class TutorialTx : MonoBehaviour
{
    // 출력할 튜토리얼 텍스트
    public Text currentTx;
    private string showTx;
    private bool isShow = false;

    public void setText(string Des)
    {
        showTx = Des;
        isShow = true;
    }

    private void Update()
    {
        if (isShow)
        {
            currentTx.text = "";
            var dotTx = currentTx.DOText(showTx, 1);
            dotTx.SetDelay(0.5f); // 스타트 출력 딜레이
            dotTx.onComplete = showConsole;
            dotTx.Play();
        }
    }

    //private void Update()
    //{
    //    if (Input.GetKeyDown(KeyCode.A))
    //    {
    //        currentTx.text = "";
    //        var dotTx = currentTx.DOText("이것은 오징어 입니다.", 1);
    //        //dotTx.SetDelay(3f);
    //        dotTx.onComplete = showConsole;
    //        dotTx.Play();
    //    }

    //    if (Input.GetKeyDown(KeyCode.S))
    //    {
    //        currentTx.text = "";
    //        var dotTx = currentTx.DOText("꿈빛 파티시엘", 1);
    //        //dotTx.SetDelay(2f);
    //        dotTx.onComplete = showConsole2;
    //        dotTx.Play();
    //    }
    //}

    public void showConsole() => Debug.Log("show1");
    //public void showConsole2() => Debug.Log("show2");
}
