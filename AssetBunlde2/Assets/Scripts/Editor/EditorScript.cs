using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

public class EditorScript : MonoBehaviour
{
    // Start is called before the first frame update
    private static void Start()
    {

    }

    [MenuItem("配置文件/生成GameSetting")]
    private static void BuildSetting()
    {
        ScriptableObject gameSetting = ScriptableObject.CreateInstance<GameSetting>();

        string pathName = "Assets/Scripts/GameSetting/GameSetting.asset";

        AssetDatabase.CreateAsset(gameSetting, pathName);
        AssetDatabase.SaveAssets();
    }
}
