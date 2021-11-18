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
            //BuildPipeline.BuildAssetBundles(Application.streamingAssetsPath, BuildAssetBundleOptions.None, BuildTarget.StandaloneWindows);
            //return;
            listAssetBundleBuild.Clear();
            this.CreateAssetBundle("Assets/Resources");
            Debug.LogError("打包结束");
            //for (int i = 0; i < listAssetBundleBuild.Count; i++)
            //{
            //    Debug.LogError("AAAA"+listAssetBundleBuild[i].assetBundleName);
            //    for (int j = 0; j < listAssetBundleBuild[i].assetNames.Length; j++)
            //    {
            //        Debug.LogError(listAssetBundleBuild[i].assetNames[j]);
            //    }
            //} 
            BuildPipeline.BuildAssetBundles(Application.streamingAssetsPath, listAssetBundleBuild.ToArray(), BuildAssetBundleOptions.None, BuildTarget.StandaloneWindows);
        }
    }
    /// <summary>
    /// 打包AssetBundle
    /// </summary>
    private void CreateAssetBundle(string path)
    {
        string[] guids = AssetDatabase.FindAssets("", new string[] { path });
        List<string> filePath = new List<string>();
        for (int i = 0; i < guids.Length; i++)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(guids[i]);
            if (AssetDatabase.IsValidFolder(assetPath))
            {
                CreateAssetBundle(assetPath);
            }
            else
            {
                filePath.Add(assetPath);
            }
        }
        if (filePath.Count <= 0)
            return;
        AssetBundleBuild assetBundleBuild = CreateAssetBundleBuild(path, filePath.ToArray());
        listAssetBundleBuild.Add(assetBundleBuild);
    }
    int index = 0;
    private AssetBundleBuild CreateAssetBundleBuild(string buildPath, string[] assetNames)
    {
        index++;
        AssetBundleBuild assetBundleBuild = new AssetBundleBuild
        {
            assetBundleName = buildPath,
            assetNames = new string[] { buildPath },
        };
        return assetBundleBuild;
    }
}
