using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloseScrollView : MonoBehaviour
{
    public GameObject HowtoPlay;

    private void Start()
    {
        HowtoPlay.gameObject.SetActive(false);
    }

    public void OpenHowtoPlay()
    {
        HowtoPlay.gameObject.SetActive(true);
    }

    public void closeHowToPlay()
    {
        HowtoPlay.gameObject.SetActive(false);
    }
}
