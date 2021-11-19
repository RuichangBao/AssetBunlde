using System.IO;
using System;
using System.Security.Cryptography;
using System.Text;


public class FileIO
{
    public static bool Exists(string path)
    {
        return File.Exists(path);
    }
    public static void CreateDirectory(string path)
    {
        if (Directory.Exists(path))
            return;
        Directory.CreateDirectory(path);
    }
    public static void Delete(string path, bool recursive = false)
    {
        if (Exists(path))
            File.Delete(path);
        if(Directory.Exists(path))
            Directory.Delete(path, recursive);
    }
    public static string GetMD5HashFromFile(string fileName)
    {
        try
        {
            FileStream file = new FileStream(fileName, FileMode.Open);
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
            throw new Exception("获取MD5失败" + ex.Message);
        }
    }
}