using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

public class FileIO
{

#if UNITY_STANDALONE
    public static readonly string platform_path = "/windows/";
    public static readonly string platform = "windows";
#elif UNITY_ANDROID
    public static readonly string platform = "android";
    public static readonly string platform_path = "/android/";
#elif UNITY_IPHONE
    public static readonly string platform = "ios";
    public static readonly string platform_path = "/ios/";
#endif

    /// <summary>
    /// 是不是在编辑器模式下，在编辑器模式下读取Resources文件夹下文件
    /// </summary>
    public static bool isEditor = false;


    private static AssetBundleManifest assetBundleManifest;
    /// <summary>
    /// ab资源缓存
    /// </summary>
    private static Dictionary<string, AssetBundle> map_ab = new Dictionary<string, AssetBundle>();

    /// <summary>
    /// 精灵缓存
    /// </summary>
    private static Dictionary<string, Sprite> map_sprite = new Dictionary<string, Sprite>();


    /// <summary>
    /// 加载Srpite
    /// </summary>
    /// <param name="spriteIndex"></param>
    /// <returns></returns>
    public static Sprite LoadSprite(int spriteIndex)
    {
        string spriteName = R.SpritePack.path[spriteIndex];
        Sprite sprite = LoadSprite(spriteName);
        if (sprite == null)
        {
            Debug.LogError("加载的图片为空");
        }
        return sprite;
    }

    public static Sprite LoadSprite(string spriteName)
    {
        if (isEditor)
        {
            return Resources.Load<Sprite>(spriteName);
        }

        Sprite sprite = null;

        if (map_sprite.ContainsKey(spriteName))
        {
            sprite = map_sprite[spriteName];
            if (sprite != null)
                return sprite;
            else
            {
                map_sprite.Remove(spriteName);
            }
        }
        else  //缓存图片不存在
        {
            string[] urls = spriteName.Split('/');
            string abName = spriteName.Substring(0, spriteName.Length - 1 - (urls[urls.Length - 1]).Length);
            Debug.LogError("abName     :"+ abName);
            AssetBundle ab = LoadAssetBundle(abName);
            if (ab == null)
            {
                Debug.LogError("加载ab包为空");
                return sprite;
            }
            Texture2D text = ab.LoadAsset<Texture2D>(urls[urls.Length - 1]);
            sprite = Sprite.Create(text, new Rect(0.0f, 0.0f, text.width, text.height), new Vector2(0.5f, 0.5f), 100.0f);
            map_sprite.Add(spriteName,sprite);
        }
        
        return sprite;
    }

    public static AssetBundle LoadAssetBundle(string abName)
    {
        if (map_ab.ContainsKey(abName))
        {
            if (map_ab[abName] != null)
                return map_ab[abName];
            else
                map_ab.Remove(abName);
        }

        AssetBundleManifest assetBundleManifest = LoadAssetBundleManifest();
        string abUrl = Application.streamingAssetsPath + platform_path + abName.ToLower();
        AssetBundle ab = AssetBundle.LoadFromFile(abUrl);
        if (ab == null)
        {
            Debug.LogError("本地加载ab包错误 加载出来的AB包为空");
            return null;
        }
        map_ab.Add(abName, ab);
        return null;
    }

    public static AssetBundleManifest LoadAssetBundleManifest()
    {
        if (assetBundleManifest == null)
        {
            string path = Application.streamingAssetsPath+ platform_path+ platform;
            Debug.LogError("加载依赖文件 path:" + path);
            
            AssetBundle manifestBundle = AssetBundle.LoadFromFile(path);
            assetBundleManifest = manifestBundle.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
            manifestBundle.Unload(false);
        }
        return assetBundleManifest;
    }


















    /// <summary>
    /// 创建不存在的文件夹
    /// </summary>
    /// <param name="url"></param>
    public static void CreateNoAreFolder(string url)
    {
        if (!Directory.Exists(url))
        {
            Directory.CreateDirectory(url);
        }
    }

    public static void CreateNoAreFile(string url, string fileName)
    {
        CreateNoAreFolder(url);
        if (File.Exists(url + @"/" + fileName))
        {
            return;
        }
        else
        {
            File.Create(url + @"/" + fileName);
        }
    }

    public static void WriteFileText(string url, string str)
    {
        File.WriteAllText(url, str);
    }
}