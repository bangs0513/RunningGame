using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStartButton : MonoBehaviour
{
    public GameObject startbutton;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(StartGameButton());
    }

    IEnumerator StartGameButton()
    {

        yield return new WaitForSeconds(7f);


        startbutton.SetActive(true);


    }
}
