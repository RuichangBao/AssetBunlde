using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

public class FileIO
{
    /// <summary>
    /// 是不是在编辑器模式下，在编辑器模式下读取Resources文件夹下文件
    /// </summary>
    private static bool isEditor = true;

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

        return sprite;
    }

    public static Sprite LoadSprite(string spriteName)
    {
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
        if (isEditor)
        {
            sprite = Resources.Load<Sprite>(spriteName);
            map_sprite.Add(spriteName,sprite);
            return sprite;
        }
        return sprite;
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

    public static void WriteFileText(string url,string str)
    {
        File.WriteAllText(url,str);
    }
}