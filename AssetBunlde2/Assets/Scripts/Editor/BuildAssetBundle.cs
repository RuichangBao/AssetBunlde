using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class BuildAssetBundle : EditorWindow
{
    [MenuItem("AssetBundle/打包")]
    private static void AssetBundle()
    {
        BuildAssetBundle window = (BuildAssetBundle)GetWindow(typeof(BuildAssetBundle));
        window.Show();
    }
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
            Debug.LogError("打包AssundleBundle");
        }
    }
    private void CreateAssetBundleName()
    {
        Debug.LogError("生成AssetBundle名字");
    }
}
fds的说法范德萨范德萨
    测试提交
