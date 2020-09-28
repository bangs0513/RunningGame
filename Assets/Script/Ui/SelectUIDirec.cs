using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectUIDirec : MonoBehaviour
{
   public Image Option;



    public void OptionButtonClick()
    {
        Option.gameObject.SetActive(true);

        Time.timeScale = 0f;
    }


    public void OptionButtonExit()
    {
        Option.gameObject.SetActive(false);


        Time.timeScale = 1f;
    }
}
