﻿using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Init : MonoBehaviour
{
    public Canvas canvas;
    void Start()
    {
        //GameObject uiLogin = Resources.Load<GameObject>("Login/UILogin");
        //GameObject UILogin = Instantiate(uiLogin, Vector3.zero, Quaternion.identity, canvas.transform);
        //UILogin.transform.localPosition = Vector3.zero;
        GameObject Cube = AssetBundleUtil.Instance.LoadGameObject("Cube");
        GameObject gameObject = Instantiate(Cube);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
