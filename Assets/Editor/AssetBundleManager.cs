using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.IO;
using System.Text;

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

    private static StringBuilder[] stringBuilder_index = new StringBuilder[BundleType.Length];
    private static StringBuilder[] stringBuilder_url = new StringBuilder[BundleType.Length];
    private static Dictionary<string, Dictionary<string, string>> sb_allName = new Dictionary<string, Dictionary<string, string>>();


    [MenuItem("BRC/生成索引并打包")]
    public static void CreateAssetBundle()
    {

        FileIO.CreateNoAreFile(dataPath + @"/Gen", "R.cs");

        dataPath = Application.dataPath;
        streamingAssetsPath = Application.streamingAssetsPath;
        sb_allName.Clear();

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
        WriteIndex();
        string outputPath = Path.Combine(pathURL, GetPlatformFolder());
        FileIO.CreateNoAreFolder(outputPath);
        BuildPipeline.BuildAssetBundles(outputPath, BuildAssetBundleOptions.ChunkBasedCompression, EditorUserBuildSettings.activeBuildTarget);
        //ReadFolder("");
        AssetDatabase.Refresh(); //刷新编辑器
    }

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
            int bundleTypeIndex = 0;
            switch (bundleType)
            {
                case "Prefab":
                    bundleTypeIndex= 0;
                    break;
                case "SpritePack":
                    bundleTypeIndex = 1;
                    break;
                case "Texture":
                    bundleTypeIndex =2;
                    break;
                case "AudioClip":
                    bundleTypeIndex = 3;
                    break;
                case "Data":
                    bundleTypeIndex = 4;
                    break;
                case "Font":
                    bundleTypeIndex = 5;
                    break;
                case "UI":
                    bundleTypeIndex = 6;
                    break;
                case "Materials":
                    bundleTypeIndex = 7;
                    break;
                default:
                    break;
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

        string resUrl = dataPath + @"/Res/" + url;

        //该目录下ab包的输出路径
        string abUrl = streamingAssetsPath + @"/" + GetPlatformFolder() + @"/" + url;
        abUrl = abUrl.ToLower();
        //Debug.LogError("输出路径abUrl:" + abUrl);
        //FileIO.CreateNoAreFolder(abUrl);


        DirectoryInfo directoryInfo = new DirectoryInfo(resUrl);
        DirectoryInfo[] directoryInfos = directoryInfo.GetDirectories();
        if (directoryInfos == null || directoryInfos.Length <= 0)
        {
            Debug.LogError("该文件夹下为空");
            return;
        }
        foreach (DirectoryInfo directory in directoryInfos)
        {
            //directory.Name ab包的名字 
            string abResUrl = resUrl + @"/" + directory.Name;
            DirectoryInfo abDirectory = new DirectoryInfo(abResUrl);

            //读取需要打包的文件 所以文件格式必须是 BundleType/包名/资源
            FileInfo[] fileInfos = abDirectory.GetFiles();
            foreach (FileInfo fileInfo in fileInfos)
            {
                if (fileInfo.Name.EndsWith(".meta"))
                {
                    continue;
                }
                //设置单个文件ab包的名字
                string file = @"Assets/Res/" + url + @"/" + abDirectory.Name + @"/" + fileInfo.Name;
                string abName = url + @"/" + abDirectory.Name;
                AssetImporter importer = AssetImporter.GetAtPath(file);
                importer.assetBundleName = abName;
                //importer.assetBundleVariant = directory.Name;
            }
        }
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
        string str = "public class R \n{\n\n}";
        FileIO.WriteFileText(Application.dataPath + @"/Gen/R.cs", str);

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
        AssetDatabase.Refresh();
    }
}