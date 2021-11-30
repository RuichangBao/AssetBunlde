using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using UnityEditor;
using UnityEngine;

public class AssetBundleUtil : Single<AssetBundleUtil>
{
    private Dictionary<string, AssetBundle> dictAssetBundle = new Dictionary<string, AssetBundle>();
    private Dictionary<string, GameObject> dictPerfab = new Dictionary<string, GameObject>();
    private Dictionary<string, Sprite> dictSprite = new Dictionary<string, Sprite>();
    private AssetBundleManifest assetBundleManifest;
    public override void Init()
    {
        base.Init();
        string streamingAssetsAbPath = Path.Combine(AssetBundleData.outPutPath, "StreamingAssets");
        AssetBundle streamingAssetsAb = AssetBundle.LoadFromFile(streamingAssetsAbPath);
        assetBundleManifest = streamingAssetsAb.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
        streamingAssetsAb.Unload(false);
    }

    public GameObject LoadGameObject(string name)
    {
        if (dictPerfab.ContainsKey(name))
            return dictPerfab[name];
        string assetBundleName = GetAssetBundleName(name);
        AssetBundle assetBundle = LoadAssetBundle(assetBundleName);
        GameObject obj = assetBundle.LoadAsset<GameObject>(name);
        dictPerfab.Add(name, obj);

        
        return obj;
    }

    public Sprite LoadSprite(string name)
    {
        if (dictSprite.ContainsKey(name))
            return dictSprite[name];
        string assetBundleName = GetAssetBundleName(name);
        AssetBundle assetBundle = LoadAssetBundle(assetBundleName);
        Sprite sprite = assetBundle.LoadAsset<Sprite>(name);
        dictSprite.Add(name, sprite);
        return sprite;
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
    public string GetAssetBundlePath(string assetBundleName)
    {
        return AssetBundleData.outPutPath + "/" + assetBundleName;
    }

    public void Unload(AssetBundle assetBundle,bool unloadAllLoadedObjects)
    {
        assetBundle.Unload(unloadAllLoadedObjects);
        if(dictAssetBundle.ContainsKey(assetBundle.name))
        {
            dictAssetBundle.Remove(assetBundle.name);
        }
    }

    /// <summary>
    /// 获取依赖assetBundle 的所有assetBundle名字
    /// </summary>
    public List<string> GetDepend(AssetBundle assetBundle)
    {
        ///1:被依赖的ab   
        Dictionary<string, List<string>> dictDepend = new Dictionary<string, List<string>>();
        foreach (AssetBundle item in dictAssetBundle.Values)
        {
            string[] abNames = assetBundleManifest.GetAllDependencies(item.name);
            if (abNames != null && abNames.Length > 0)
            {
                for (int i = 0; i < abNames.Length; i++)
                {
                    string abName = abNames[i];
                    if (dictDepend.ContainsKey(abName))
                    {
                        List<string> listDepend = dictDepend[abName];
                        if (listDepend.Contains(item.name))
                        {
                            continue;
                        }
                        else
                        {
                            listDepend.Add(item.name);
                        }
                    }
                    else
                    {
                        dictDepend.Add(abName, new List<string>() { item.name });
                    }
                }
            }
        }
        if (dictDepend.ContainsKey(assetBundle.name))
        {
            return dictDepend[assetBundle.name];
        }
        return null;
    }

    public void Test()
    {
        string[]strs = assetBundleManifest.GetDirectDependencies("login");
        Debug.LogError(strs.Length);
        for (int i = 0; i < strs.Length; i++)
        {
            Debug.LogError(strs[i]);
        }
    }
}