using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Respawn : MonoBehaviour
{
    [SerializeField,Header("리스폰 될 위치(save)")]
    private Transform respawnPos;

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
            transform.position = respawnPos.position;
    }
}
