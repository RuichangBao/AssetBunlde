using System;
using System.Collections;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class Test : MonoBehaviour
{


    void Start()
    {
        string url = Application.streamingAssetsPath + "/windows/brc";
        Debug.LogError(url);
        FileIO.DeleteFile(url);
        AssetDatabase.Refresh(); //刷新编辑器
    }

}