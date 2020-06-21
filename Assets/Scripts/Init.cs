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
    // Use this for initialization
    void Start()
    {
        ///远程资源md5码
        ///key：md5码
        ///value：路径
        Dictionary<string, string> map_resMd5Network = new Dictionary<string, string>();

        //服务器所有md5码路径
        string md5Url = "http://192.168.1.115/ResMd5.txt";
        string resUrl = "http://192.168.1.115";
        //远程MD5码
        string netWorkRes = FileIO.GetTextByNetwork(md5Url);
        string[] netWorkMd5s = netWorkRes.Split('\n');
        foreach (string str in netWorkMd5s)
        {
            if (!string.IsNullOrEmpty(str))
            {
                string[] netWorkMd5 = str.Split('|');
                if (netWorkMd5 == null || netWorkMd5.Length < 2)
                    continue;
                if (string.IsNullOrEmpty(netWorkMd5[0]))
                {
                    Debug.LogError("错误：Md5码为空");
                    continue;
                }
                if (string.IsNullOrEmpty(netWorkMd5[1]))
                {
                    Debug.LogError("错误：资源路径为空");
                    continue;
                }
                netWorkMd5[0] = netWorkMd5[0].Trim();
                netWorkMd5[1] = netWorkMd5[1].Trim();
                if (map_resMd5Network.ContainsKey(netWorkMd5[0]))
                {
                    Debug.LogError("错误已经包含该资源:" + netWorkMd5[0]);
                    continue;
                }
                map_resMd5Network.Add(netWorkMd5[0].Trim(), netWorkMd5[1]);
            }
        }
        foreach (string key in map_resMd5Network.Keys)
        {
            string url = resUrl + "/" + map_resMd5Network[key];
            string urlPath = Application.streamingAssetsPath + "/" + map_resMd5Network[key];
            //FileIO.WWWGet(url, urlPath);
        }


        SceneManager.LoadScene("Test");
    }
}