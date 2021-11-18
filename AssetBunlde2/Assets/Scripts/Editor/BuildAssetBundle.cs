using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class BuildAssetBundle : EditorWindow
{
    
    private void OnGUI()
    {
        if (GUILayout.Button("生成AssetBundle名字", GUILayout.Width(200)))
        {
            CreateAssetBundleName();
        }
        if (GUILayout.Button("清除AssetBundle名字", GUILayout.Width(200)))
        {
            Debug.LogError("清除AssetBundle名字");
        }
        if (GUILayout.Button("打包AssundleBundle", GUILayout.Width(200)))
        {
            this.CreateAssetBundle();
        }
    }
    private void CreateAssetBundleName()
    {
        Debug.LogError("生成AssetBundle名字");
    }

    /// <summary>
    /// 打包AssetBundle
    /// </summary>
    private void CreateAssetBundle()
    {
        Debug.LogError("打包AssundleBundle");
    }

    private AssetBundleBuild CreateAssetBundleBuild(string buildPath, string[] assetNames)
    {
        AssetBundleBuild assetBundleBuild = new AssetBundleBuild
        {
            assetBundleName = buildPath,
            assetNames = assetNames,
        };
        return assetBundleBuild;
    }
}
