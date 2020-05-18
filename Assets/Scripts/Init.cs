using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class Init : MonoBehaviour
{
    //资源地址
    string url = @"http://192.168.2.15/Windows.rar";
    string path;
    // Use this for initialization
    void Start()
    {
        path = Application.streamingAssetsPath + "/Windows";
        Debug.LogError("path:" + path);
        FileIO.CreateNoAreFolder(path);
        //SceneManager.LoadScene("Test");
    }
}