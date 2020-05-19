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
    private string url = "http://192.168.2.15/ResMd5.txt";
    private Dictionary<string, string> map_resMd5Network;
    // Use this for initialization
    void Start()
    {
        //string url = Application.dataPath + @"/Gen/ResMd5.txt";
        Debug.LogError(url);
        map_resMd5Network =FileIO.GetResMd5ByNetwork(url);
        //SceneManager.LoadScene("Test");
    }
}