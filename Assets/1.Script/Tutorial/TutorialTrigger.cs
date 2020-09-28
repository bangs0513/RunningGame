using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class TutorialTrigger : MonoBehaviour
{
    [SerializeField, Header("튜토리얼 말풍선")]
    private GameObject speechBubble;
    [SerializeField, Header("튜토리얼 스크린")]
    private GameObject TutorialPauseScreen;
    [SerializeField, Header("아무키나 입력하세요 텍스트")]
    private GameObject PressAnyKeyTx;
    [SerializeField, Header("대사 텍스트")]
    private Text DesTx;
    [SerializeField, Header("고유번호")]
    private int id;
    [SerializeField, Header("대사출력 시간"), Range(1,5)]
    private float[] time;
    private int totalIndex;

    private DesciptionData desData; // 대사 데이터

    private bool isShow = false;
    private bool isPauseTutorial; // 튜토리얼 일시정지
    private bool isAnyKey = false;

    private void Awake()
    {
        desData = GameObject.Find("DesManager").GetComponent<DesciptionData>();
        totalIndex = time.Length;
    }

    private void Update()
    {
        setAtiveTutorialPause();
    }

    private void OnTriggerEnter(Collider other)
    {
        // 플레이어가 트리거를 밟을 시
        if (other.CompareTag("Player"))
        {
            print("튜토리얼트리거");
            speechBubble.SetActive(true);
            isPauseTutorial = true;
            TutorialPauseScreen.SetActive(true);
            //showPressAnyKey();
            StartCoroutine(showDescription());
        }
    }

    private void setAtiveTutorialPause()
    {
        if (isPauseTutorial)
        {
            Time.timeScale = 0;
            if (isAnyKey)
            {
                if (Input.anyKeyDown)
                {
                    if (GameManager.Instance.uiController.isPause) return; // 일시정지가 켜져있으면 애니키를 동작할 수 없게함
                    Time.timeScale = 1;
                    isPauseTutorial = false;
                    speechBubble.SetActive(false);
                    TutorialPauseScreen.SetActive(false);
                    PressAnyKeyTx.SetActive(false);
                    isShow = false;
                    isAnyKey = false;
                    gameObject.SetActive(false);
                }
            }
        }
    }

    // Anykey
    public void showPressAnyKey()
    {
        print("아무버튼");
        PressAnyKeyTx.SetActive(true);
        isAnyKey = true;
    }

    private int index = 0;

    // 닷트윈 애니
    IEnumerator showDescription()
    {
        print("대사 실행");
        string des = desData.GetDesciption(id, index);
        DesTx.text = "";
        var dotTx = DesTx.DOText(des, time[index]);
        dotTx.SetUpdate(true);
        dotTx.SetDelay(0.5f);

        if (index != totalIndex - 1)
        {
            yield return new WaitForSecondsRealtime(time[index] + 1f);
            index++;
            StartCoroutine(showDescription());
        }
        else
        {
            print("끝날분기");
            dotTx.onComplete = showPressAnyKey;
            index = 0;
            dotTx.Play();
        }

        #region test
        // 직접 타이핑 효과
        //IEnumerator showDescription()
        //{
        //    if (index == 0f)
        //        yield return new WaitForSecondsRealtime(0.5f);

        //    print("대사 실행");
        //    string des = desData.GetDesciption(id, index);
        //    if (des != null)
        //    {
        //        DesTx.text = "";
        //        for (int i = 0; i <= des.Length; i++)
        //        {
        //            DesTx.text = des.Substring(0, i);
        //            yield return new WaitForSecondsRealtime(0.07f);
        //        }
        //        index++;
        //        yield return new WaitForSecondsRealtime(1f);
        //        StartCoroutine(showDescription());
        //    }
        //    else
        //    {
        //        print("끝날분기");
        //        showPressAnyKey();
        //        index = 0;
        //    }
        //}

        #endregion
    }
}
