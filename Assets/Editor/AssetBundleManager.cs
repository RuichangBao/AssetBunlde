using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.IO;

public class AssetBundleManager
{
    private static DirectoryInfo directory;
    private static string dataPath;
    private static string streamingAssetsPath;
    public static string pathURL = Application.streamingAssetsPath + "/";

    /// <summary>
    /// 能打包的类型
    /// </summary>
    public static readonly string[] BundleType = new string[]{
        "Prefab",
        "SpritePack",
        "Texture",
        "AudioClip",
        "Data",
        "Font",
        "UI",
        "Materials"
    };

    [MenuItem("BRC/生成索引并打包")]
    public static void CreateAssetBundle()
    {
        dataPath = Application.dataPath;
        streamingAssetsPath = Application.streamingAssetsPath;

        directory = new DirectoryInfo(dataPath + @"/Res");
        DirectoryInfo[] directoryInfos = directory.GetDirectories();
        if (directoryInfos == null || directoryInfos.Length <= 0)
        {
            return;
        }
        foreach (DirectoryInfo directoryInfo in directoryInfos)
        {
            ReadFolder(directoryInfo.Name);
        }

        //ReadFolder("");
        AssetDatabase.Refresh(); //刷新编辑器
    }
    /// <summary>
    /// 判断是不是打包类型
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    private static bool IsBunldeType(string type)
    {
        int i = 0;
        while (i < BundleType.Length)
        {
            if (BundleType[i] == type)
                return true;
            i++;
        }
        return false;
    }

    /// <summary>
    /// 读取文件夹
    /// </summary>
    /// <param name="url"></param>
    /// <returns></returns>
    private static DirectoryInfo[] ReadFolder(string url)
    {
        directory = new DirectoryInfo(dataPath + @"/Res/" + url);
        DirectoryInfo[] directoryInfos = directory.GetDirectories();
        if (directoryInfos == null || directoryInfos.Length <= 0)
            return null;
        foreach (DirectoryInfo directoryInfo in directoryInfos)
        {
            Debug.Log("读取文件夹:" + directoryInfo.Name);
            if (IsBunldeType(directoryInfo.Name))
            {
                ReadFile(url + @"/" + directoryInfo.Name);
            }
            else
            {
                ReadFolder(url + @"/" + directoryInfo.Name);
            }
        }
        return directoryInfos;
    }

    /// <summary>
    /// 读取文件
    /// </summary>
    private static void ReadFile(string url)
    {
        string abUrl = streamingAssetsPath + @"/" + GetPlatformFolder() + @"/" + url; //该目录下ab包的输出路径
        string resUrl = dataPath + @"/Res/" + url;

        DirectoryInfo directoryInfo = new DirectoryInfo(resUrl);
        DirectoryInfo[] directoryInfos = directoryInfo.GetDirectories();
        Debug.LogError(directoryInfos.Length);
        if (directoryInfos == null || directoryInfos.Length <= 0)
        {
            Debug.LogError("该文件夹下为空");
            return;
        }
        FileIO.CreateNoAreFolder((abUrl).ToLower());//创建AB包文件夹
        foreach (DirectoryInfo directory in directoryInfos)
        { //directory.Name ab包的名字 
            string abResUrl = resUrl + @"/" + directory.Name;
            DirectoryInfo abDirectory = new DirectoryInfo(abResUrl);
            FileInfo[] fileInfos = abDirectory.GetFiles();

            foreach (FileInfo fileInfo in fileInfos)
            {
                Debug.LogError("resUrl:" + resUrl);
                Debug.LogError(resUrl + @"/" + fileInfo.Name);
                Debug.LogError(@"Assets/Res/" + url + @"/" + fileInfo.Name);
                AssetImporter importer = AssetImporter.GetAtPath(@"Assets/Res/" + url);
                importer.assetBundleName = directory.Name;
                importer.assetBundleVariant = directory.Name;
            }

            BuildPipeline.BuildAssetBundles(abUrl, BuildAssetBundleOptions.None, EditorUserBuildSettings.activeBuildTarget);
        }

    }

    /// <summary>
    /// 根据编译器运行的环境生成对应的文件夹
    /// </summary>
    /// <returns></returns>
    private static string GetPlatformFolder()
    {
        switch (EditorUserBuildSettings.activeBuildTarget)
        {
            case BuildTarget.iOS:
                return "Ios";
            case BuildTarget.Android:
                return "Android";
            case BuildTarget.StandaloneWindows:
            case BuildTarget.StandaloneWindows64:
                return "Windows";
        }
        return "";
    }

    [MenuItem("BRC/生成AssetBunlde名字")]
    public static void CreateAssetBundleName()
    {
        //太坑了 AssetImporter是基于Assets的上层目录的 这样写不正确
        //string url = Application.streamingAssetsPath + @"/Notice/11";

        //AssetImporter importer = AssetImporter.GetAtPath("Assets/Res");
        AssetImporter importer = AssetImporter.GetAtPath("Assets/Res/BRC/Building/SpritePack");
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
            AssetDatabase.RemoveAssetBundleName(assetBundleName, true);
        }
        AssetDatabase.Refresh(); //刷新编辑器 
    }

    [MenuItem("BRC/测试程序")]
    public static void Test44()
    {
        string RScriptsUrl= Application.dataPath + @"/Scripts";
        //FileIO.CreateNoAreFolder(RScriptsUrl);
        //FileIO.CreateNoAreFile(RScriptsUrl, "R.cs");
        /*string str = GetPlatformFolder();
        Debug.LogError(str);

        Debug.LogError(EditorUserBuildSettings.activeBuildTarget);
        Debug.LogError(EditorUserBuildSettings.activeBuildTarget.ToString());
        //选中的文件夹或者文件
        Object[] selects = Selection.objects;
        foreach (Object selected in selects)
        {
            string path = AssetDatabase.GetAssetPath(selected);
            Debug.LogError("path:" + path);
            AssetImporter assetImporter = AssetImporter.GetAtPath(path);
            assetImporter.assetBundleName = selected.name;
            Debug.LogError("selected.name:" + selected.name);
            assetImporter.assetBundleVariant = "unity";
            //assetImporter.SaveAndReimport();   
        }*/
        AssetDatabase.Refresh();
    }
}