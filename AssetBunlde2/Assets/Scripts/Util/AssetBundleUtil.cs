using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using UnityEngine;

public class AssetBundleUtil : Single<AssetBundleUtil>
{
    private Dictionary<string, AssetBundle> dictAssetBundle = new Dictionary<string, AssetBundle>();
    private Dictionary<string, GameObject> dictPerfab = new Dictionary<string, GameObject>();
    private AssetBundleManifest assetBundleManifest;
    public override void Init()
    {
        base.Init();
        string streamingAssetsAbPath = Path.Combine(AssetBundleData.outPutPath, "StreamingAssets");
        AssetBundle streamingAssetsAb = AssetBundle.LoadFromFile(streamingAssetsAbPath);
        assetBundleManifest = streamingAssetsAb.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
    }

    public GameObject LoadGameObject(string name)
    {
        if (dictPerfab.ContainsKey(name))
            return dictPerfab[name];
        string assetBundleName = GetAssetBundleName(name);
        AssetBundle assetBundle = LoadAssetBundle(assetBundleName);
        Debug.LogError(assetBundle.name);
        for(int i=0; i<assetBundle.AllAssetNames().Length;i++)
        {
            Debug.LogError(assetBundle.AllAssetNames()[i]);
        }
        GameObject obj = assetBundle.LoadAsset<GameObject>(name);
        return obj;
    }
    private AssetBundle LoadAssetBundle(string assetBundleName)
    {
        AssetBundle assetBundle;
        if (!dictAssetBundle.ContainsKey(assetBundleName))
        {
            string assetBundlePath = GetAssetBundlePath(assetBundleName);
            assetBundle = AssetBundle.LoadFromFile(assetBundlePath);
            dictAssetBundle.Add(assetBundleName, assetBundle);
        }
        else
        {
            assetBundle = dictAssetBundle[assetBundleName];
        }
        
        string[] allDependencies = assetBundleManifest.GetAllDependencies(assetBundleName);
        for (int i = 0; i < allDependencies.Length; i++)
        {
            string dependenciesName = allDependencies[i];
            if (!dictAssetBundle.ContainsKey(dependenciesName))
            {
                string assetBundlePath = GetAssetBundlePath(dependenciesName);
                AssetBundle dependenciesAssetBundle = AssetBundle.LoadFromFile(assetBundlePath);
                dictAssetBundle.Add(dependenciesName, dependenciesAssetBundle);
            }
        }
        return assetBundle;
    }

    private string GetAssetBundleName(string assetName)
    {
        XmlDocument AssetBundleConf = new XmlDocument();
        AssetBundleConf.Load(AssetBundleData.AssetBundleConf);

        XmlNode xn = AssetBundleConf.SelectSingleNode("AssetBundle");
        XmlNodeList xnl = xn.ChildNodes;
        foreach (XmlNode xnf in xnl)
        {
            XmlElement xe = (XmlElement)xnf;
            if (assetName == xe.GetAttribute("assetName"))
                return (xe.GetAttribute("assetBundleName"));
        }
        return null;
    }
    private string GetAssetBundlePath(string assetBundleName)
    {
        return AssetBundleData.outPutPath + "/" + assetBundleName;
    }
}