using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UiController : MonoBehaviour
{
    [SerializeField,Header("우편물")]
    private Text mailTx; // 우편물
    [SerializeField, Header("미션 달성률")]
    private Text missionTx;
    [Header("미션 결과")]
    public Text missionResultTx;
    [SerializeField, Header("현재 플레이어 이동속도")]
    private Text testTx;
    [SerializeField, Tooltip("현재 씬 우편물 시스템 개수")]
    private float culMissonMax = 0;

    [SerializeField]
    private Player player;

    public float mailCount { get; set; } = 5; // 우편물 개수
    public float missionCount { get; set; } = 0; // 미션 성공 수

    private void Awake()
    {
        // 게임이 시작될 때 우편물 시스템 존이 몇개 있는 지 파악
        GameObject[] misson = GameObject.FindGameObjectsWithTag("Misson");
        culMissonMax = misson.Length;
    }

    private void Update()
    {
        mailTx.text = $"우편물 : {mailCount}"; // 현재 우편물 개수 출력
        testTx.text = $"스피드 : {player.WalkSpeed}";
        MissonAchievementQuotient();
    }

    // 미션 달성률
    private void MissonAchievementQuotient()
    {
        // 현재 미션 달성률
        float culMisson = (missionCount / culMissonMax) * 100;
        culMisson = Mathf.Clamp(culMisson, 0, 100);
        string culMissonCounts = string.Format("{0:N0}", culMisson);
        missionTx.text = $"달성률 : {culMissonCounts}%"; // 현재 미션 달성률 출력
    }
}
