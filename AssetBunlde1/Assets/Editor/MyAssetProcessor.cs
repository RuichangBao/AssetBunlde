using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.IO;
using System.Text;
using System;

public class MyAssetProcessor : AssetPostprocessor
{
    private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths, bool didDomainReload)
    {
        //Debug.LogError("资源导入完成");
        TestPrint(importedAssets,"增加目录");
        TestPrint(deletedAssets, "删除资源");
        TestPrint(movedAssets, "移动资源");
        Dictionary<int, int> dic = new Dictionary<int, int>();
        bool state = dic.TryGetValue(1, out int num2);

    }
    private static void TestPrint(string[] names, string name)
    {
        if (names == null || names.Length <= 0)
            return;

        Debug.LogError(name);
        foreach (var item in names)
        {
            Debug.LogError(item);
        }
    }
}