using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using ICSharpCode.SharpZipLib.Zip;
using System;
using System.Security.Cryptography;
using System.Text;
using System.Net;

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
    /// 图集缓存
    /// </summary>
    private static Dictionary<string, Texture2D> map_texture2D = new Dictionary<string, Texture2D>();

    /// <summary>
    /// 材质缓存
    /// </summary>
    private static Dictionary<string, Material> map_material = new Dictionary<string, Material>();

    private static Dictionary<string, AudioClip> map_audioClip = new Dictionary<string, AudioClip>();

    /// <summary>
    /// 预制体缓存
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
            map_sprite.Add(spriteName, sprite);
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

        string abName = prefabName.Substring(0, prefabName.LastIndexOf('/'));

        AssetBundle ab = LoadAssetBundle(abName);
        LoadDependencies(abName);
        if (ab == null)
        {
            Debug.LogError("加载ab包为空");
            return null;
        }
        string[] urls = prefabName.Split('/');
        GameObject obj = ab.LoadAsset<GameObject>(urls[urls.Length - 1]);
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

    public static Material LoadMaterial(int materialIndex)
    {
        string materialName = R.Materials.path[materialIndex];
        Material material = LoadMaterial(materialName);
        if (material == null)
        {
            Debug.LogError("加载材质为空");
        }
        return material;
    }

    private static Material LoadMaterial(string materialName)
    {
        if (isEditor)
            return Resources.Load<Material>(materialName);
        if (map_material.ContainsKey(materialName))
        {
            if (map_material[materialName] != null)
                return map_material[materialName];
            else
                map_material.Remove(materialName);
        }

        string abName = materialName.Substring(0, materialName.LastIndexOf('/'));
        LoadDependencies(abName);
        AssetBundle ab = LoadAssetBundle(abName);


        if (ab == null)
        {
            Debug.LogError("加载材质时 加载ab为空");
            return null;
        }
        string[] urls = materialName.Split('/');
        Material material = ab.LoadAsset<Material>(urls[urls.Length - 1]);
        if (material == null)
        {
            Debug.LogError("加载材质为空");
            return null;
        }

        map_material.Add(materialName, material);
        return material;
    }

    public static AudioClip LoadAudioClip(int audioClipIndex)
    {
        string audioClipName = R.AudioClip.path[audioClipIndex];
        AudioClip audioClip = LoadAudioClip(audioClipName);
        if (audioClip == null)
        {
            Debug.LogError("加载音频为空");
        }
        return audioClip;
    }

    private static AudioClip LoadAudioClip(string audioClipName)
    {
        if (isEditor)
        {
            return Resources.Load<AudioClip>(audioClipName);
        }

        if (map_audioClip.ContainsKey(audioClipName))
        {
            if (map_audioClip[audioClipName] != null)
                return map_audioClip[audioClipName];
            else
                map_audioClip.Remove(audioClipName);
        }
        string abName = audioClipName.Substring(0, audioClipName.LastIndexOf('/'));
        AssetBundle ab = LoadAssetBundle(abName);
        if (ab == null)
        {
            Debug.LogError("加载音频时，ab包为空");
            return null;
        }

        string[] urls = audioClipName.Split('/');
        Debug.LogError(urls[urls.Length - 1]);
        AudioClip audioClip = ab.LoadAsset<AudioClip>(urls[urls.Length - 1]);
        if (audioClip == null)
        {
            Debug.LogError("加载音频为空");
            return null;
        }

        map_audioClip.Add(audioClipName, audioClip);
        return audioClip;
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

        //AssetBundleManifest assetBundleManifest = LoadAssetBundleManifest();
        //加载 Prefab+文件夹+预制体格式的ab包文件
        string abUrl = Application.streamingAssetsPath + platform_path + abName.ToLower();
        AssetBundle ab = AssetBundle.LoadFromFile(abUrl);

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
            string path = Application.streamingAssetsPath + platform_path + platform;

            AssetBundle manifestBundle = AssetBundle.LoadFromFile(path);
            assetBundleManifest = manifestBundle.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
            manifestBundle.Unload(false);
        }
        return assetBundleManifest;
    }
    /// <summary>
    /// 加载依赖(注意 不能在加载AB包的时候 加载依赖 如果 出现)
    /// </summary>
    /// <param name="abName"></param>
    public static void LoadDependencies(string abName)
    {
        AssetBundleManifest assetBundleManifest = LoadAssetBundleManifest();
        string[] dependencies = assetBundleManifest.GetAllDependencies(abName);
        if (dependencies == null || dependencies.Length <= 0)
            return;
        Debug.LogError("加载依赖abName:" + abName);
        foreach (string item in dependencies)
        {
            LoadAssetBundle(item);
        }
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
        File.WriteAllText(url, str, Encoding.UTF8);
    }

    public static void WriteFileTextAppend(string url, string str)
    {
        File.AppendAllText(url, str);
    }
    /// <summary>
    /// 获取一个文件的Md5码
    /// </summary>
    /// <param name="filePath"></param>
    /// <returns></returns>
    public static string GetFileMd5(string filePath)
    {
        try
        {
            FileStream file = new FileStream(filePath, FileMode.Open);
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] retVal = md5.ComputeHash(file);
            file.Close();
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < retVal.Length; i++)
            {
                sb.Append(retVal[i].ToString("x2"));
            }
            return sb.ToString();
        }
        catch (Exception ex)
        {
            Debug.LogError("错误信息：" + ex);
            return null;
        }
    }

    /// <summary>
    /// 下载文件
    /// </summary>
    /// <param name="url"> 下载文件地址</param>
    /// <param name="savePath">保存文件地址名字</param>
    /// <returns></returns>
    public static IEnumerator DownFile(string url, string savePath)
    {
        FileInfo file = new FileInfo(savePath);
        UnityEngine.Debug.LogError("Start:" + Time.realtimeSinceStartup);
        WWW www = new WWW(url);
        yield return www;
        if (www.isDone)//下载完成保存文件
        {
            byte[] bytes = www.bytes;
            FileStream fs = new FileStream(savePath, FileMode.Append);
            BinaryWriter bw = new BinaryWriter(fs);
            fs.Write(bytes, 0, bytes.Length);
            fs.Flush();     //流会缓冲，此行代码指示流不要缓冲数据，立即写入到文件。
            fs.Close();     //关闭流并释放所有资源，同时将缓冲区的没有写入的数据，写入然后再关闭。
            fs.Dispose();   //释放流
            www.Dispose();
        }
    }
    /// <summary>
    /// 从服务器获取文本
    /// </summary>
    /// <param name="url"></param>
    public static string GetTextByNetwork(string url)
    {
        try
        {
            WebClient client = new WebClient();
            byte[] buffer = client.DownloadData(url);
           return Encoding.UTF8.GetString(buffer);
        }
        catch(Exception e)
        {
            Debug.LogError("从服务器获取脚本错误"+e.Message);
            return null;
        }
    }
    /// <summary>
    /// 本地获取文本
    /// </summary>
    /// <param name="url"></param>
    /// <returns></returns>
    public static List<string> GetTextLocal(string url)
    {
        Debug.LogError("本地获取文本url:"+url);
        List<string> list_str = new List<string>();
     
        try
        {
            StreamReader streamReader = new StreamReader(url);
            while (!streamReader.EndOfStream)
            {
                list_str.Add( streamReader.ReadLine());
            }
            streamReader.Close();
            
        }
        catch (Exception e)
        {
            Debug.LogError("从本地获取脚本错误" + e.Message);
            return list_str;
        }
        return list_str;
    }
    /// <summary>
    /// 删除一个文件
    /// </summary>
    /// <param name="url"></param>
    public static void DeleteFile(string url)
    {
        try
        {
            FileInfo fileInfo = new FileInfo(url);
            fileInfo.Delete();
        }
        catch (Exception e)
        {
            Debug.LogError("删除文件错误"+e.Message);
        }
       
    }
}