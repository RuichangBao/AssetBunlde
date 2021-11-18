using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.IO;

public class BuildAssetBundle : EditorWindow
{
    private List<AssetBundleBuild> listAssetBundleBuild = new List<AssetBundleBuild>();
    private void OnGUI()
    {
        if (GUILayout.Button("打包AssundleBundle", GUILayout.Width(200)))
        {
            listAssetBundleBuild.Clear();
            this.CreateAssetBundle("Assets/Resources");

            AssetBundleBuild[] assetBundleBuilds = listAssetBundleBuild.ToArray();
            BuildPipeline.BuildAssetBundles(Application.streamingAssetsPath, assetBundleBuilds, BuildAssetBundleOptions.None, BuildTarget.StandaloneWindows);
            Debug.LogError("打包结束");
        }
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

            if (!AssetDatabase.IsValidFolder(assetPath))
            {
                listFilePath.Add(assetPath);
            }
        }
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
                dictBundleBuild[buildPath].Add(listFilePath[i]);
            }
            else
            {
                dictBundleBuild[buildPath] = new List<string>() { listFilePath[i] };
            }
        }
        foreach (string buildPath in dictBundleBuild.Keys)
        {
            AssetBundleBuild assetBundleBuild = CreateAssetBundleBuild(buildPath, dictBundleBuild[buildPath].ToArray());
            listAssetBundleBuild.Add(assetBundleBuild);
            if (!AssetDatabase.IsValidFolder(buildPath))
            {
                Debug.LogError("创建文件夹：  "+ buildPath);
                AssetDatabase.CreateFolder("",buildPath);
            }
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

    private AssetBundleBuild CreateAssetBundleBuild(string buildPath, string[] assetNames)
    {
        //string hash = FileIO.GetMD5HashFromFile(buildPath);
        Debug.LogError("buildPath:"+ buildPath);
        AssetBundleBuild assetBundleBuild = new AssetBundleBuild
        {
            assetBundleName = buildPath,
            assetNames = assetNames,
        };
        return assetBundleBuild;
    }
}