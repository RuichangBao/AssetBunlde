using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.IO;

public class AssetBunldeManager//:Editor
{
    private static DirectoryInfo directory;
    private static DirectoryInfo directoryFile;
    private static string dataPath;
    private static string streamingAssetsPath;

    [MenuItem("BRC/AssetBunlde资源管理/生成索引并打包-lz4压缩")]
    public static void Test()
    {
        dataPath = Application.dataPath;
        streamingAssetsPath = Application.streamingAssetsPath;

        string url = @"/Res";


        ReadFolder(url);
        AssetDatabase.Refresh(); //刷新编辑器
    }

    /// <summary>
    /// 读取文件夹
    /// </summary>
    /// <param name="url"></param>
    /// <returns></returns>
    private static DirectoryInfo[] ReadFolder(string url)
    {
        ReadFile(url);
        directory = new DirectoryInfo(dataPath + @"/" + url);
        DirectoryInfo[] directoryInfos = directory.GetDirectories();
        if (directoryInfos == null || directoryInfos.Length <= 0)
            return null;
        foreach (DirectoryInfo directoryInfo in directoryInfos)
        {
            Debug.Log("读取文件夹:" + directoryInfo.Name);
            ReadFolder(url + @"/" + directoryInfo.Name);
        }
        return directoryInfos;
    }

    /// <summary>
    /// 读取文件
    /// </summary>
    private static void ReadFile(string url)
    {
        string abUrl = streamingAssetsPath + @"/" + url;
        Debug.Log("读取文件:url：" + url);
        directoryFile = new DirectoryInfo(dataPath + @"/" + url);
        if (string.IsNullOrEmpty(dataPath + @"/" + url))
            return;
        FileInfo[] fileInfos = directoryFile.GetFiles();
        if (fileInfos == null || fileInfos.Length <= 0)
            return;
        foreach (FileInfo fileInfo in fileInfos)
        {
            if (!fileInfo.Name.EndsWith(".meta"))
            {
                Debug.Log("读取文件:url:" + url + "    fileInfo.Name:" + fileInfo.Name);

                CreateNoAreFolder(abUrl); //创建ab包文件夹
                BuildPipeline.BuildAssetBundles(abUrl, BuildAssetBundleOptions.None, BuildTarget.StandaloneWindows64);
                                          
                //BuildPipeline.BuildAssetBundles(abUrl, BuildAssetBundleOptions.None, EditorUserBuildSettings.activeBuildTarget);
            }
        }
    }




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


    [MenuItem("BRC/ATest")]
    public static void Test33()
    {
        AssetBundleBuild abBuild = new AssetBundleBuild();
        abBuild.assetBundleName = "123456"; // 就是我们要的目标AssetLabel
        abBuild.assetBundleName = "78945"; // 就是我们要的目标AssetLabel
        Debug.Log("第四次修改");

        //dfsadfa
    }
}