using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class FileIO
{
    /// <summary>
    /// 创建不存在的文件夹
    /// </summary>
    /// <param name="url"></param>
    public static void CreateNoAreFolder(string url)
    {
        if (!Directory.Exists(url))
        {
            Directory.CreateDirectory(url);
        }
    }

    public static void CreateNoAreFile(string url, string fileName)
    {
        CreateNoAreFolder(url);
        if (File.Exists(url + @"/" + fileName))
        {
            return;
        }
        else
        {
            File.Create(url + @"/" + fileName);
        }
    }

}