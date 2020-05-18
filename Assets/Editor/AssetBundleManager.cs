using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.IO;
using System.Text;
using System;

public class AssetBundleManager
{
    private static DirectoryInfo directory;
    private static string dataPath;
    private static string streamingAssetsPath;
    public static string pathURL = Application.streamingAssetsPath + "/";
    public static string editorUrl;
 

    /// <summary>
    /// 能打包的类型
    /// </summary>
    public static readonly string[] BundleType = new string[]{
        "Prefab",
        "SpritePack",
        "Texture",
        "AudioClip",
        "Materials"
    };

    private static StringBuilder[] stringBuilder_index = new StringBuilder[BundleType.Length];
    private static StringBuilder[] stringBuilder_url = new StringBuilder[BundleType.Length];
    private static Dictionary<string, Dictionary<string, string>> sb_allName = new Dictionary<string, Dictionary<string, string>>();
    private static StringBuilder resMd5StringBuilder = new StringBuilder();

    [MenuItem("BRC/生成索引并打包")]
    public static void CreateAssetBundle()
    {
        
        FileIO.CreateNoAreFile(dataPath + @"/Gen", "R.cs");

        dataPath = Application.dataPath;
        streamingAssetsPath = Application.streamingAssetsPath;
        sb_allName.Clear();
        ClearAssetBundleNames();
        directory = new DirectoryInfo(dataPath + @"/Resources");

        DirectoryInfo[] directoryInfos = directory.GetDirectories();
        if (directoryInfos == null || directoryInfos.Length <= 0)
        {
            return;
        }
        foreach (DirectoryInfo directoryInfo in directoryInfos)
        {
            ReadFolder(directoryInfo.Name);
        }
       
        if(!FileIO.isEditor)
        {
            string outputPath = Path.Combine(pathURL, GetPlatformFolder());
            FileIO.CreateNoAreFolder(outputPath);
            BuildPipeline.BuildAssetBundles(outputPath, BuildAssetBundleOptions.ChunkBasedCompression, EditorUserBuildSettings.activeBuildTarget);
        }
        WriteIndex();
        CreateAssetMD5();//给资源生成MD5码
        AssetDatabase.Refresh(); //刷新编辑器
    }
    /// <summary>
    /// 打索引
    /// </summary>
    private static void WriteIndex()
    {
        foreach (string bundleType in sb_allName.Keys)
        {
            int index = 0;
            Dictionary<string, string> sb_res = sb_allName[bundleType];
            StringBuilder sbName = new StringBuilder();
            StringBuilder sbUrl = new StringBuilder();
            sbName.Append("\n");
            foreach (string resName in sb_res.Keys)
            {
                sbName.Append("\t\tpublic const int " + resName + "=" + index+";\n");
                index++;
                sbUrl.Append("\""+sb_res[resName]+"\",");
            }
            int bundleTypeIndex = Array.IndexOf(BundleType,bundleType);
            if (bundleTypeIndex < 0)
            {
                Debug.LogError("类型错误，无法打索引");
                continue;
            }
            stringBuilder_index[bundleTypeIndex] = sbName;
            stringBuilder_url[bundleTypeIndex] = sbUrl;
        }
        StringBuilder str = new StringBuilder();
        str.Append("public class R\n{");
        for (int i = 0; i < BundleType.Length; i++)
        {
            string bundleType = BundleType[i];
            str.Append("\n\tpublic class "+bundleType+"\n\t{");
            str.Append(stringBuilder_index[i]);
            str.Append("\n\t\tpublic static string[] path = {");
            str.Append(stringBuilder_url[i]);
            str.Append("};\n\t}");
        }
        str.Append("\n}");
        FileIO.WriteFileText(dataPath + @"/Gen/R.cs",str.ToString());

        //stringBuilder_index.
        //stringBuilder_url.cle
        sb_allName.Clear();
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
        directory = new DirectoryInfo(dataPath + @"/Resources/" + url);
        DirectoryInfo[] directoryInfos = directory.GetDirectories();
        if (directoryInfos == null || directoryInfos.Length <= 0)
            return null;
        foreach (DirectoryInfo directoryInfo in directoryInfos)
        {
            if (IsBunldeType(directoryInfo.Name))
            {
                ReadFile(url + @"/" + directoryInfo.Name, directoryInfo.Name);
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
    private static void ReadFile(string url, string bundleType)
    {

        string resUrl = dataPath + @"/Resources/" + url;

        //该目录下ab包的输出路径
        string abUrl = streamingAssetsPath + @"/" + GetPlatformFolder() + @"/" + url;
        abUrl = abUrl.ToLower();



        DirectoryInfo directoryInfo = new DirectoryInfo(resUrl);
       

        /*****************************读取当前文件夹下的文件夹，一个文件夹打成一个ab包 Start*************************************/
        DirectoryInfo[] directoryInfos = directoryInfo.GetDirectories();
        if (directoryInfos == null || directoryInfos.Length <= 0)
        {
            return;
        }
        
        foreach (DirectoryInfo directory in directoryInfos)
        {
            //directory.Name ab包的名字 
            string abResUrl = resUrl + @"/" + directory.Name;
            DirectoryInfo abDirectory = new DirectoryInfo(abResUrl);

            //读取需要打包的文件 所以文件格式必须是 BundleType/包名/资源
            FileInfo[] fileInfos2 = abDirectory.GetFiles();
            foreach (FileInfo fileInfo in fileInfos2)
            {
                if (fileInfo.Name.EndsWith(".meta"))
                {
                    continue;
                }
                if (!FileIO.isEditor)
                { //设置单个文件ab包的名字
                    string file = @"Assets/Resources/" + url + @"/" + abDirectory.Name + @"/" + fileInfo.Name;
                    string abName = url + @"/" + abDirectory.Name;
                    AssetImporter importer = AssetImporter.GetAtPath(file);
                    importer.assetBundleName = abName;
                }
                string fileName = fileInfo.Name.Substring(0, fileInfo.Name.LastIndexOf('.'));
                AddAResIndex(bundleType, (abDirectory.Name + "_" + fileName).ToUpper(), url + @"/" + abDirectory.Name + @"/" + fileName);
            }
        }
        /*****************************读取当前文件夹下的文件夹，一个文件夹打成一个ab包 End*************************************/


        /*****************************读取当前文件夹下的文件，单独打成ab包 Start*************************************/
        FileInfo[] fileInfos = directoryInfo.GetFiles();
        foreach (FileInfo fileInfo in fileInfos)
        {
            if (fileInfo.Name.EndsWith(".meta"))
            {
                continue;
            }
            string fileName = fileInfo.Name.Substring(0, fileInfo.Name.LastIndexOf('.'));
           
            if (!FileIO.isEditor)
            { //设置单个文件ab包的名字
                string file = @"Assets/Resources/" + url + @"/" + fileInfo.Name;
                Debug.LogWarning("该文件夹下的文件不能被加载 "+file);
                string abName = url + @"/" + fileName;
                AssetImporter importer = AssetImporter.GetAtPath(file);
                importer.assetBundleName = abName;
            }
            AddAResIndex(bundleType, fileName.ToUpper(), url + @"/" + fileName);
        }
        /*****************************读取当前文件夹下的文件，单独打成ab包 End*************************************/

    }


    private static void AddAResIndex(string bundleType,string name,string url)
    {
        if (!sb_allName.ContainsKey(bundleType))
        {
            sb_allName.Add(bundleType, new Dictionary<string, string>());
        }
        Dictionary<string, string> sbDic = sb_allName[bundleType];
        if (sbDic == null)
        {
            sbDic = new Dictionary<string, string>();
            sb_allName.Add(bundleType, sbDic);
        }
        if (sbDic == null)
        {
            Debug.LogError(sbDic);
        }
        sbDic.Add(name,url);
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
                return "ios";
            case BuildTarget.Android:
                return "android";
            case BuildTarget.StandaloneWindows:
            case BuildTarget.StandaloneWindows64:
                return "windows";
        }
        return "";
    }

    /// <summary>
    /// 清除AssetBunlde名字
    /// </summary>
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
        //AssetDatabase.Refresh(); //刷新编辑器 
    }
    /// <summary>
    /// 给资源生成md5码用于资源比对更新
    /// </summary>
    [MenuItem("BRC/生成md5")]
    public static void CreateAssetMD5()
    {
        dataPath = Application.dataPath;
        FileIO.CreateNoAreFile(dataPath + @"/Gen", "ResMd5.txt");
        streamingAssetsPath = Application.streamingAssetsPath;
        resMd5StringBuilder.Remove(0, resMd5StringBuilder.Length);
        CreateAssetMD5(streamingAssetsPath);

        FileIO.WriteFileText(dataPath + @"/Gen/ResMd5.txt", resMd5StringBuilder.ToString());
        AssetDatabase.Refresh(); //刷新编辑器
    }


    private static void CreateAssetMD5(string url)
    {
        DirectoryInfo directoryInfo = new DirectoryInfo(url);
        FileInfo[] fileInfos= directoryInfo.GetFiles();
        DirectoryInfo[] directoryInfos= directoryInfo.GetDirectories();
        

        if (fileInfos != null && fileInfos.Length > 0)
        {
            foreach (FileInfo fileInfo in fileInfos)
            {
                if (!fileInfo.Name.EndsWith(".meta"))
                {
                    resMd5StringBuilder.Append(FileIO.GetFileMd5(url + "/" + fileInfo.Name));
                    resMd5StringBuilder.Append("|");
                    resMd5StringBuilder.Append(url + @"/" + fileInfo.Name);
                    resMd5StringBuilder.Append("\n");
                }
            }
        }

        if (directoryInfos != null && directoryInfos.Length > 0)
        {
            foreach (DirectoryInfo item in directoryInfos)
            {
                if (item != null)
                {
                    CreateAssetMD5(url + "/" + item.Name);
                }
            }      
        }
        return;
    }

    [MenuItem("BRC/测试程序")]
    public static void Test44()
    {
        int[] nums = new int[] { 1, 2, 3, 4, 5, 6 };

        Debug.LogError(Array.IndexOf(nums,1));
        Debug.LogError(Array.IndexOf(nums, 100));
        string str = "dafsdaf.dasfadsf.dsafsafasdf";
        Debug.LogError(str.IndexOf('.'));
        Debug.LogError(str.LastIndexOf('.'));
        //FileIO.CreateNoAreFolder(RScriptsUrl);

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
        //AssetDatabase.Refresh();
    }
}