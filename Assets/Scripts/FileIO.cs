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
    public static bool isEditor = true;


    private static AssetBundleManifest assetBundleManifest;
    /// <summary>
    /// ab资源缓存
    /// </summary>
    private static Dictionary<string, AssetBundle> map_ab = new Dictionary<string, AssetBundle>();

    /// <summary>
    /// 精灵缓存
    /// </summary>
    private static Dictionary<string, Sprite> map_sprite = new Dictionary<string, Sprite>();

    private static Dictionary<string, Texture2D> map_texture2D = new Dictionary<string, Texture2D>();

    /// <summary>
    /// ab包缓存
    /// </summary>
    private static Dictionary<string, GameObject> map_obj = new Dictionary<string, GameObject>();

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

    public static GameObject LoadPrefab(int prefabIndex)
    {
        string prefabName = R.Prefab.path[prefabIndex];
        GameObject prefab = LoadPrefab(prefabName);
        if (prefab == null)
        {
            Debug.LogError("加载预制体为空");
        }
        return prefab;
    }

    public static GameObject LoadPrefab(string prefabName)
    {
        if (isEditor)
        {
            return Resources.Load<GameObject>(prefabName);
        }

        if (map_obj.ContainsKey(prefabName))
        {
            if (map_obj[prefabName] != null)
            {
                return map_obj[prefabName];
            }
            else
            {
                map_obj.Remove(prefabName);
            }
        }
       
        AssetBundle ab = LoadAssetBundle(prefabName.Substring(0, prefabName.LastIndexOf('/')));
        if (ab == null)
        {
            Debug.LogError("加载ab包为空");
            return null;
        }
        string[] urls = prefabName.Split('/');
        GameObject obj = ab.LoadAsset<GameObject>(urls[urls.Length-1]);
        if (obj == null)
        {
            Debug.LogError("加载出的预制体为空");
            return null;
        }
        map_obj.Add(prefabName, obj);
        return obj;
    }

    public static Texture2D LoadTexture2D(int textureIndex)
    {
        string textureName = R.Texture.path[textureIndex];
        Texture2D texture2D = LoadTexture2D(textureName);
        if (texture2D == null)
        {
            Debug.LogError("加载Texture2D为空");
        }
        return texture2D;
    }

    public static Texture2D LoadTexture2D(string textureName)
    {
        if (isEditor)
        {
            return Resources.Load<Texture2D>(textureName);
        }
        if (map_texture2D.ContainsKey(textureName))
        {
            if (map_texture2D[textureName] != null)
            {
                return map_texture2D[textureName];
            }
            else
                map_texture2D.Remove(textureName);
        }

        AssetBundle ab = LoadAssetBundle(textureName.Substring(0, textureName.LastIndexOf('/')));
        if (ab == null)
        {
            Debug.LogError("加载Texture2D时 加载ab为空");
            return null;
        }
        string[] urls = textureName.Split('/');
        Texture2D texture2D = ab.LoadAsset<Texture2D>(urls[urls.Length - 1]);
        if (texture2D == null)
        {
            Debug.LogError("加载Texture2D为空");
            return null;
        }

        map_texture2D.Add(textureName, texture2D);
        return texture2D;
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
        //加载 Prefab+文件夹+预制体格式的ab包文件
        string abUrl = Application.streamingAssetsPath + platform_path + abName.ToLower();
        AssetBundle ab= AssetBundle.LoadFromFile(abUrl);

        if (ab != null)
        {
            map_ab.Add(abName, ab);
            return ab;
        }
        Debug.LogError("加载Ab包为空");
        return null;
    }

    public static AssetBundleManifest LoadAssetBundleManifest()
    {
        if (assetBundleManifest == null)
        {
            string path = Application.streamingAssetsPath+ platform_path+ platform;
            
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