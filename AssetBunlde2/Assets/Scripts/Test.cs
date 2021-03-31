using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Test : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        GameSetting gameSetting = AssetDatabase.LoadAssetAtPath<GameSetting>("Assets/Scripts/GameSetting/GameSetting.asset");
        Debug.LogError(gameSetting.a);
        gameSetting.a = 1000;
        AssetDatabase.SaveAssets();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
