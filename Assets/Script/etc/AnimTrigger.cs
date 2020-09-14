using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

public class AnimTrigger : MonoBehaviour
{
    [SerializeField, Header("애니메이션을 활성화 시킬 오브젝트")]
    private GameObject act;

    private void OnTriggerEnter(Collider other)
    {
        // 플레이어가 트리거를 밟을 시
        if (other.CompareTag("Player"))
        {
            print("트리거");
            act.GetComponent<Animator>().enabled = true;
        }
    }
}
