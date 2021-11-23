using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.IO;
using System.Xml;

public class BuildAssetBundle : EditorWindow
{
    private List<AssetBundleBuild> listAssetBundleBuild = new List<AssetBundleBuild>();
    private AssetBundleManifest assetBundleManifest;
    private void OnGUI()
    {
        if (GUILayout.Button("打包AssundleBundle", GUILayout.Width(200)))
        {
            listAssetBundleBuild.Clear();
            this.CreateAssetBundle(AssetBundleData.inPutPath); 
           
            FileIO.Delete(AssetBundleData.outPutPath + "/", true);
            FileIO.CreateDirectory(AssetBundleData.outPutPath);
            AssetBundleBuild[] assetBundleBuilds = listAssetBundleBuild.ToArray();
            BuildPipeline.BuildAssetBundles(AssetBundleData.outPutPath, assetBundleBuilds, BuildAssetBundleOptions.None, BuildTarget.StandaloneWindows);
            SaveAssetBundleConf();
            Debug.LogError("打包结束");
            AssetDatabase.Refresh();
            
        }
        if (GUILayout.Button("清理所有AB", GUILayout.Width(200)))
        {
            FileIO.Delete(AssetBundleData.outPutPath + "/", true);
            AssetDatabase.Refresh();
        }
    }
    private void SaveAssetBundleConf()
    {
        if (FileIO.Exists(AssetBundleData.AssetBundleConf))
        {
            FileIO.Delete(AssetBundleData.AssetBundleConf);
        }
        for (int i = 0; i < listAssetBundleBuild.Count; i++)
        {
            AssetBundleBuild assetBundleBuild = listAssetBundleBuild[i];
            for (int j = 0; j < assetBundleBuild.assetNames.Length; j++)
            {
                SaveAssetBundleConf(assetBundleBuild.assetNames[j], assetBundleBuild.assetBundleName);
            }
        }
    }
    /// <summary>
    /// 保存AssetBundleConf文件
    /// </summary>
    private void SaveAssetBundleConf(string assetName, string assetBundleName)
    {
        XmlDocument xmlDocument = new XmlDocument();//实例化文档对象
        if (FileIO.Exists(AssetBundleData.AssetBundleConf))//如果文件已存在，载入文档
        {
            xmlDocument.Load(AssetBundleData.AssetBundleConf);
        }
        else//否则
        {
            XmlElement xmlElement = xmlDocument.CreateElement("AssetBundle");//加入根节点
            xmlDocument.AppendChild(xmlElement);
        }

        XmlElement student = xmlDocument.CreateElement("AssetBundle");

        string name = GetNameByPath(assetName);
        student.SetAttribute("assetName", name);
        student.SetAttribute("assetBundleName", assetBundleName);
        xmlDocument.DocumentElement.AppendChild(student);
        xmlDocument.Save(AssetBundleData.AssetBundleConf);
    }

    private string GetNameByPath(string path)
    {
        int startIndex = path.LastIndexOf('/');

        if (startIndex > 0)
        {
            path = path.Substring(startIndex + 1, path.Length - (startIndex + 1));
        }
        int endIndex = path.IndexOf('.');
        if (endIndex > 0)
        {
            path = path.Substring(0, endIndex);
        }
        return path;
    }

    /// <summary>
    /// 打包AssetBundle
    /// </summary>
    private void CreateAssetBundle(string path)
    {
        string[] guids = AssetDatabase.FindAssets("", new string[] { path });
        List<string> listFilePath = new List<string>();
        for (int i = 0; i < guids.Length; i++)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(guids[i]);

            if (AssetDatabase.IsValidFolder(assetPath))
            {   //文件夹排除
                continue;
            }
            if (assetPath.EndsWith(".cs"))
            {   //排除cs脚本
                continue;
            }
            listFilePath.Add(assetPath);
        }

        //处理依赖 Start
        //key:被依赖的资源 value所有依赖该资源的资源
        Dictionary<string, List<string>> dictDependencies = new Dictionary<string, List<string>>();
        for (int i = 0; i < listFilePath.Count; i++)
        {
            string filePath = listFilePath[i];
            string[] dependencies = AssetDatabase.GetDependencies(filePath);
            for (int j = 0; j < dependencies.Length; j++)
            {
                string depend = dependencies[j];
                if (filePath == depend)//排除自己
                {
                    continue;
                }
                if (dictDependencies.ContainsKey(depend))
                {
                    List<string> listDepend = dictDependencies[depend];
                    if (!listDepend.Contains(filePath))
                    {
                        listDepend.Add(filePath);
                    }
                }
                else
                {
                    dictDependencies.Add(depend, new List<string>() { listFilePath[i] });
                }
            }
        }
        foreach (string item in dictDependencies.Keys)
        {
            if (dictDependencies.Count > 1)
            {
                listFilePath.Add(item);
            }
        }
        //处理依赖 End

        listFilePath = CopyListString(listFilePath);
        if (listFilePath.Count <= 0)
            return;
       
        Dictionary<string, List<string>> dictBundleBuild = new Dictionary<string, List<string>>();
        for (int i = 0; i < listFilePath.Count; i++)
        {
            int lastIndex = listFilePath[i].LastIndexOf('/');
            string buildPath = listFilePath[i].Substring(0, lastIndex);
            if (dictBundleBuild.ContainsKey(buildPath))
            {
                List<string> listBundleBuild = dictBundleBuild[buildPath];
                listBundleBuild.Add(listFilePath[i]);
                //dictBundleBuild[buildPath].Add(listFilePath[i]);
            }
            else
            {
                List<string> listBundleBuild = new List<string>() { listFilePath[i] };
                dictBundleBuild.Add(buildPath, listBundleBuild);
            }
        }
        Dictionary<string, AssetBundleBuild> dictAssetBundleBuild = new Dictionary<string, AssetBundleBuild>();
        foreach (string buildPath in dictBundleBuild.Keys)
        {
            string assetBundleName = buildPath;
            AssetBundleBuild assetBundleBuild = CreateAssetBundleBuild(assetBundleName, dictBundleBuild[buildPath].ToArray());
            dictAssetBundleBuild.Add(assetBundleBuild.assetBundleName, assetBundleBuild);
            if (!AssetDatabase.IsValidFolder(buildPath))
            {
                AssetDatabase.CreateFolder("", buildPath);
            }
        }

        foreach (AssetBundleBuild assetBundleBuild in dictAssetBundleBuild.Values)
        {
            listAssetBundleBuild.Add(assetBundleBuild);
        }
    }

    private List<string> CopyListString(List<string> list1)
    {
        List<string> list2 = new List<string>();
        for (int i = 0; i < list1.Count; i++)
        {
            string str1 = list1[i];
            bool have = false;
            for (int j = 0; j < list2.Count; j++)
            {
                if (str1 == list2[j])
                {
                    have = true;
                    break;
                }
            }
            if (!have)
            {
                list2.Add(str1);
            }
        }
        return list2;
    }

    private AssetBundleBuild CreateAssetBundleBuild(string assetBundleName, string[] assetNames)
    {
        //string hash = FileIO.GetMD5HashFromFile(buildPath);
        AssetBundleBuild assetBundleBuild = new AssetBundleBuild
        {
            assetBundleName = assetBundleName,
            assetNames = assetNames,
        };
        return assetBundleBuild;
    }

    private void LoadManifest(string path)
    {
        AssetBundleManifest manifest = null;
        AssetBundle.UnloadAllAssetBundles(true);
        AssetBundle bundle = AssetBundle.LoadFromFile(path);
        if (bundle == null)
        {
            Debug.LogError("[AssetBundlePatcher:LoadManifest] Load AssetBundleManifest failure");
            return;
        }
        assetBundleManifest = bundle.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
    }
}