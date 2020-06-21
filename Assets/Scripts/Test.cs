using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class Test : MonoBehaviour
{

    public Image image;
    public Button button;
    int i = 0;
    void Start()
    {
        button.onClick.AddListener(BtnOnClick);
    }

    private void BtnOnClick()
    {
        if (i < R.SpritePack.path.Length)
            i++;
        else
            i = 0;
        image.sprite = FileIO.LoadSprite(i);
    }
}