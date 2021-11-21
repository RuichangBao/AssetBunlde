using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Init : MonoBehaviour
{
    public Canvas canvas;
    void Start()
    {
        GameObject Cube = AssetBundleUtil.Instance.LoadGameObject("Cube");
        GameObject gameObject = Instantiate(Cube);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
