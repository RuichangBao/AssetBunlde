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
        //远程资源md5码
        Dictionary<string, string> map_resMd5Network = new Dictionary<string, string>();
        //本地资源md5码
        Dictionary<string, string> map_resMd5 = new Dictionary<string, string>();

        //服务器所有md5码路径
        string md5Url = "http://192.168.2.15/ResMd5.txt";
        string resUrl = "http://192.168.2.15";
        //远程MD5码
        string netWorkRes = FileIO.GetTextByNetwork(md5Url);
        string[] netWorkMd5s = netWorkRes.Split('\n');
        foreach (string str in netWorkMd5s)
        {
            if (!string.IsNullOrEmpty(str))
            {
                string[] netWorkMd5 = str.Split('|');
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
                if (map_resMd5Network.ContainsKey(netWorkMd5[0]))
                {
                    Debug.LogError("错误已经包含该资源:"+ netWorkMd5[0]);
                    continue;
                }
                map_resMd5Network.Add(netWorkMd5[0], netWorkMd5[1]);
            }
        }

        //本地Md5码
        List<string> list_md5s = FileIO.GetTextLocal(Application.dataPath + @"/Gen/" + "ResMd5.txt");
        foreach (string str in list_md5s)
        {
            string[] netWorkMd5 = str.Split('|');
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
            if (map_resMd5.ContainsKey(netWorkMd5[0]))
            {
                Debug.LogError("错误已经包含该资源:"+ netWorkMd5[0]);
                continue;
            }
            map_resMd5.Add(netWorkMd5[0], netWorkMd5[1]);
        }


        foreach (string item in map_resMd5.Values)
        {
            if (!map_resMd5Network.ContainsValue(item))
            {   //删除不需要的资源
                FileIO.DeleteFile(Application.streamingAssetsPath + "/" + item);
            }
        }

        foreach (string item in map_resMd5Network.Keys)
        {
            if (!map_resMd5.ContainsKey(item))
            {
                StartCoroutine(FileIO.DownFile(resUrl + "/" + map_resMd5Network[item], Application.streamingAssetsPath + "/" + map_resMd5Network[item]));
            }
        }

       

        //SceneManager.LoadScene("Test");
    }
}