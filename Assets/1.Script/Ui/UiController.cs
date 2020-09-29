using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UiController : MonoBehaviour
{
    #region 우편물 관련
    [SerializeField,Header("우편물")]
    private Text mailTx; // 우편물
    [SerializeField, Tooltip("현재 씬 우편물 시스템 개수")]
    private float culMissonMax = 0;
    public float mailCount { get; set; } = 5; // 우편물 개수
    #endregion

    #region 미션 관련
    [SerializeField, Header("미션 달성률")]
    private Text missionTx;
    [SerializeField, Header("미션 달성률 바")]
    private Image missionBar;
    [SerializeField, Header("미션 아이콘")]
    private Transform missionIcon;
    [Header("미션 결과")]
    public Text missionResultTx;
    public float missionCount { get; set; } = 0; // 미션 성공 수
    private float curMisson; // 현재 스테이지 미션 달성률
    private float prexx = -199; // 미션 아이콘 제어용
    #endregion

    #region 타임스코어 관련
    [SerializeField, Header("타임스코어 텍스트")]
    private Text timeScroeTx;
    private float timeScroe; // 타임스코어
    #endregion

    #region 스크린
    // 일시정지
    [SerializeField, Header("일시정지 스크린")]
    private GameObject PauseScreen;
    public bool isPause { get; set; }   // 일시정지 체크

    // 클리어
    [SerializeField, Header("결과 스크린")]
    public GameObject ResultScreen;
    [SerializeField, Header("클리어성공UI")]
    private GameObject Clear;
    [SerializeField, Header("클리어실패UI")]
    private GameObject Fail;
    [SerializeField]
    private Text resultTimeTx; // 걸린 시간
    [SerializeField]
    private Text resultMissonTx; // 미션 결과
    #endregion

    [SerializeField, Header("현재 씬 이름")]
    private string SceneName;
    // 테스트용
    [SerializeField, Header("현재 플레이어 이동속도")]
    private Text testTx;

    private void Awake()
    {
        // 게임이 시작될 때 우편물 시스템 존이 몇개 있는 지 파악
        GameObject[] misson = GameObject.FindGameObjectsWithTag("Misson");
        culMissonMax = misson.Length;
        GameManager.Instance.uiController = this;
    }

    private void Update()
    {
        mailTx.text = $"{mailCount}"; // 현재 우편물 개수 출력
        testTx.text = $"스피드 : {GameManager.Instance.player.WalkSpeed}";
        MissonAchievementQuotient();
        SetMissonBar();
        TimeScore();
        GamePause(); // 일시정지
    }

    // 미션 달성률 이미지와 아이콘
    private void SetMissonBar()
    {
        // 현재 Hp를 0~1 사이의 수로 표현
        float culMissonSuccess = missionCount / culMissonMax;

        culMissonSuccess = Mathf.Clamp(culMissonSuccess, 0, 1);

        // 게이지 증가 감소
        if (missionBar != null)
            missionBar.fillAmount = Mathf.Lerp(missionBar.fillAmount, culMissonSuccess, 2 * Time.deltaTime);

        // 미션아이콘 처리
        float rexx = Mathf.Lerp(-199, 331, culMissonSuccess);
        prexx = Mathf.Lerp(prexx, rexx, Time.deltaTime*2f);
        missionIcon.localPosition = new Vector3(34, prexx, 0);
    }

    // 미션 달성률 텍스트
    private void MissonAchievementQuotient()
    {
        // 현재 미션 달성률
        curMisson = (missionCount / culMissonMax) * 100;
        curMisson = Mathf.Clamp(curMisson, 0, 100);
        string culMissonCounts = string.Format("{0:N0}", curMisson);
        missionTx.text = $"{culMissonCounts}%"; // 현재 미션 달성률 출력
    }

    // 타임 스코어 출력
    private void TimeScore()
    {
        var playerStatus = GameManager.Instance.player.playerStatus;
        if (playerStatus == Player.PlayerStatus.CLEAR || playerStatus == Player.PlayerStatus.DEAD) return;
        timeScroe += Time.deltaTime;
        string culTimeCounts = timeScroe.ToString("00.00");
        culTimeCounts = culTimeCounts.Replace(".", ":");
        timeScroeTx.text = $"<b>{culTimeCounts}</b>"; // 현재 타임스코어 출력
    }

    public void ShowResult()
    {
        ResultScreen.SetActive(true);
        if (curMisson >= 60)
        {
            print("sucsses");
            Clear.SetActive(true);
        }
        else
        {
            print("Fail");
            Fail.SetActive(true);
        }
        resultTimeTx.text = timeScroeTx.text;
        resultMissonTx.text = missionTx.text;
    }

    public void GameOver()
    {
        ResultScreen.SetActive(true);
        Fail.SetActive(true);
        resultTimeTx.text = timeScroeTx.text;
        resultMissonTx.text = missionTx.text;
    }

    private void GamePause()
    {
        // esc 버튼 클릭 시 일시정지
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            // 일시 정지 활성화
            if (!isPause)
            {
                PauseScreen.SetActive(true);
                Time.timeScale = 0;
                isPause = true;
                return;
            }
        }
    }

    #region Button 삽입용
    // continue
    public void GameReturn()
    {
        // 일시정지 창 나가기
        if (isPause)
        {
            PauseScreen.SetActive(false);
            Time.timeScale = 1;
            isPause = false;
            return;
        }
    }

    public void ReStart()
    {
        //GameManager.Instance.isGameOver = true; // 게임오버 처리
        if (isPause)
        {
            Time.timeScale = 1;
            isPause = false;
        }
        SceneManager.LoadScene(SceneName);
    }

    public void Quit()
    {
        if (isPause)
        {
            Time.timeScale = 1;
            isPause = false;
        }
        LoadingManager.instance.beforeSceneSetup(SceneName);
    } 
    #endregion
}
