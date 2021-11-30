using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class Init : MonoBehaviour
{
    public Canvas canvas;
    public Image image1;
    public Image image2;
    public Image image3;
    void Start()
    {
        //GameObject Cube = AssetBundleUtil.Instance.LoadGameObject("Cube");
        //GameObject gameObject = Instantiate(Cube);
        Sprite sprite1 = AssetBundleUtil.Instance.LoadSprite("chuanwu");
        image1.sprite = sprite1;
        Sprite sprite2 = AssetBundleUtil.Instance.LoadSprite("fangzhigongfang");
        image2.sprite = sprite2;
        Sprite sprite3 = AssetBundleUtil.Instance.LoadSprite("guantouchang");
        image3.sprite = sprite3;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
