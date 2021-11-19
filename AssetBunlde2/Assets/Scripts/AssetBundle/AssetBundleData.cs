using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class AssetBundleData
{
    public static string inPutPath = Path.Combine("Assets","Resources");//打包输入目录
    public static string outPutPath = Application.streamingAssetsPath;//打包输出目录
    public static string AssetBundleConf = outPutPath + "/AssetBundleConf.xml";
}
