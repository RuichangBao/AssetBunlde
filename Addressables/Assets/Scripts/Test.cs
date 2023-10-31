using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;

public class Test : MonoBehaviour
{
    public AssetReference baseCube;
    public Button btn;
    // Start is called before the first frame update
    void Start()
    {
        btn.onClick.AddListener(CreateObj);
    }

    private void CreateObj()
    {
        baseCube.LoadAssetAsync<GameObject>().Completed += LoadCompleted;
        AsyncOperationHandle<GameObject> obj = baseCube.InstantiateAsync();
        
    }

    private void LoadCompleted(AsyncOperationHandle<GameObject> handle)
    {
        Debug.LogError("º”‘ÿÕÍ≥…");
    }

}
