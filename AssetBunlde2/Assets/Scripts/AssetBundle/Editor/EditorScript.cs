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
        GameObject obj = GameObject.Find("Cube");
        GameObject obj2 = GameObject.Find("Cube1");
        for (int i = 0; i < 100; i++)
        {
            GameObject objw = Instantiate(obj2);
            objw.transform.SetParent(obj.transform);
            objw.transform.localPosition = new Vector3(i/100f, i / 100f, i / 100f);
            objw.name = (i+1).ToString();
        }
    }
}
