using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.IO;

public class AssetBundleManager
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


    [MenuItem("BRC/生成AssetBunlde名字")]
    public static void CreateAssetBundle()
    {
        //太坑了 AssetImporter是基于Assets的上层目录的 这样写不正确
        //string url = Application.streamingAssetsPath + @"/Notice/11";
        AssetImporter importer = AssetImporter.GetAtPath("Assets/Res");
        importer.assetBundleName = "测试生成AssetBunlde名字/1245/dfgfd";
        importer.assetBundleVariant = "测试生成AssetBundle变量";
        AssetDatabase.Refresh(); //刷新编辑器 
    }


    [MenuItem("BRC/清除AssetBunlde名字")]
    public static void ClearAssetBundleNames()
    {
        string[] assetBundleNames = AssetDatabase.GetAllAssetBundleNames();
        int assetBundleNameLength = assetBundleNames.Length;
        for (int i = assetBundleNameLength - 1; i >= 0; i--)
        {
            string assetBundleName = assetBundleNames[i];
            AssetDatabase.RemoveAssetBundleName(assetBundleName,true);
        }
        AssetDatabase.Refresh(); //刷新编辑器 
    }

    [MenuItem("BRC/测试程序")]
    public static void Test44()
    {
        //选中的文件夹或者文件
        Object[] selects = Selection.objects;
        foreach (Object selected in selects)
        {
            string path = AssetDatabase.GetAssetPath(selected);
            Debug.LogError("path:"+ path);
            AssetImporter assetImporter = AssetImporter.GetAtPath(path);
            assetImporter.assetBundleName = selected.name;
            Debug.LogError("selected.name:"+ selected.name);
            assetImporter.assetBundleVariant = "unity";
            //assetImporter.SaveAndReimport();   
        }
        AssetDatabase.Refresh();
    }
}