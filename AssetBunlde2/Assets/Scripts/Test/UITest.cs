using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// 测试场景用的代码
/// </summary>
public class UITest : MonoBehaviour
{

    public Button btnCreateAb;
    public Button btnCreateAss;
    public Button btnCreateObj;
    public Button btnDeleteAssets1;
    public Button btnDeleteAssets2;
    public Button btnDeleteObj;
    public Button btnUnloadTrue;
    public Button btnUnloadFalse;
    private string abName = "login";
    private AssetBundle assetBundle;
    private GameObject obj;
    private List<GameObject> listObj = new List<GameObject>();
    void Start()
    {
        btnCreateAb.onClick.AddListener(BtnCreateAbClick);
        btnCreateAss.onClick.AddListener(Btn2OnClick);
        btnCreateObj.onClick.AddListener(Btn3OnClick);
        btnDeleteAssets1.onClick.AddListener(()=> { DestroyingAssets(true); });
        btnDeleteAssets2.onClick.AddListener(() => { DestroyingAssets(false); });
        btnDeleteObj.onClick.AddListener(DeleteObjOnClick);
        btnUnloadTrue.onClick.AddListener(()=> { Unload(true); });
        btnUnloadFalse.onClick.AddListener(() => { Unload(false); });
        Resources.UnloadUnusedAssets();
    }
    /// <summary>
    /// 加载Ab
    /// </summary>
    private void BtnCreateAbClick()
    {
        string assetBundlePath = AssetBundleUtil.Instance.GetAssetBundlePath(abName);
        assetBundle = AssetBundle.LoadFromFile(assetBundlePath);
    }
    /// <summary>
    /// 从AssetBundle里边创建obj
    /// </summary>
    private void Btn2OnClick()
    {
        obj = assetBundle.LoadAsset<GameObject>("Assets/Resources/AssetBundle/Login/Cube.prefab");

    }
    /// <summary>
    /// 实例化obj
    /// </summary>
    private void Btn3OnClick()
    {
        GameObject obj = Instantiate(this.obj);
        listObj.Add(obj);
    }
    /// <summary>
    /// 删除obj
    /// </summary>
    private void DestroyingAssets(bool allowDestroyingAssets)
    {
        DestroyImmediate(obj, allowDestroyingAssets);
    }

    private void DeleteObjOnClick()
    {
        while (listObj.Count > 0)
        {
            Destroy(listObj[0]);
            listObj.RemoveAt(0);
        }
    }

    private void Unload(bool unloadAllLoadedObjects)
    {
        assetBundle.Unload(unloadAllLoadedObjects);
    }
  
    // Update is called once per frame
    void Update()
    {

    }
    [ContextMenu("设置变量")]
    public void OnEditorAssignVarible()
    {
        btnCreateAb=GameObject.Find("Canvas/btnCreateAb").GetComponent<Button>();
        btnCreateAss = GameObject.Find("Canvas/btnCreateAss").GetComponent<Button>();
        btnCreateObj = GameObject.Find("Canvas/btnCreateObj").GetComponent<Button>();
        btnDeleteAssets1 = GameObject.Find("Canvas/btnDeleteAssets1").GetComponent<Button>();
        btnDeleteAssets2 = GameObject.Find("Canvas/btnDeleteAssets2").GetComponent<Button>();
        btnDeleteObj = GameObject.Find("Canvas/btnDeleteObj").GetComponent<Button>();
        btnUnloadTrue = GameObject.Find("Canvas/btnUnloadTrue").GetComponent<Button>();
        btnUnloadFalse = GameObject.Find("Canvas/btnUnloadFalse").GetComponent<Button>();
    }
}