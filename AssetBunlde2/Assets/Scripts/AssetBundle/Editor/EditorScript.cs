using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

public class EditorScript : EditorWindow
{
    [MenuItem("配置文件/生成GameSetting")]
    private static void BuildSetting()
    {
        ScriptableObject gameSetting = ScriptableObject.CreateInstance<GameSetting>();

        string pathName = "Assets/Scripts/GameSetting/GameSetting.asset";

        AssetDatabase.CreateAsset(gameSetting, pathName);
        AssetDatabase.SaveAssets();
    }

    [MenuItem("AssetBundle/打包")]
    private static void AssetBundle()
    {
        BuildAssetBundle window = (BuildAssetBundle)GetWindow(typeof(BuildAssetBundle));
        window.Show();
    }

    [MenuItem("AssetBundle/测试")]
    private static void Test()
    {
        AssetDatabase.CreateFolder("", "Assets/ss/dff/sss");
        Debug.LogError("测试内容");
    }
}
