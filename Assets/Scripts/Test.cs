using System;
using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class Test : MonoBehaviour
{


    void Start()
    {
        StartCoroutine(FileIO.DownFile("http://192.168.2.15/windows/windows", @"F:\Users\Administrator\AssetBunlde\Assets\StreamingAssets\windows\windows"));
    }
    public static IEnumerator Aest()
    {
        Debug.LogError("VBVVVV");
        yield return new WaitForSeconds(5);
        Debug.LogError("AAAAAAAAAA");
    }
}