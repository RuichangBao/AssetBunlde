using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class LoadManager : MonoBehaviour
{  
    // 可寻址资源的弱引用
    public AssetReference cubeRef;
    private void Start()
    {
        // 回调形式
        //LoadGameObject();
        //LoadGameObjectCallBack();
        //InstantiateGameObject();

        // 异步形式
        //AsyncLoadCube();
        //AsyncInstantiateCube();

        //this.RefLoadCube();

        Addressables.InitializeAsync();
        // 同步加载
        InstantiatePrefab();
    }

    #region 回调形式

    /// <summary>
    /// 加载物体 
    /// 逻辑简单且不需复用，直接使用Lambda表达式的形式
    /// </summary>
    private void LoadGameObject()
    {
        // 参数："Cube" 为可寻址系统的地址
        Addressables.LoadAssetAsync<GameObject>("Cube").Completed += (obj) =>
        {
            GameObject go = obj.Result;

            Instantiate(go, Vector3.zero, Quaternion.identity);
        };
    }

    /// <summary>
    /// 加载物体
    /// </summary>
    private void LoadGameObjectCallBack()
    {
        Addressables.LoadAssetAsync<GameObject>("Assets/Prefabs/Sphere.prefab").Completed += LoadCallBack;
    }

    /// <summary>
    /// 加载物体的回调函数
    /// </summary>
    private void LoadCallBack(AsyncOperationHandle<GameObject> handle)
    {
        GameObject go = handle.Result;
        Instantiate(go, Vector3.right * 2, Quaternion.identity);
    }

    /// <summary>
    /// 加载并实例化物体
    /// </summary>
    private void InstantiateGameObject()
    {
        Addressables.InstantiateAsync("Assets/Prefabs/Capsule.prefab").Completed += (obj) =>
        {
            // 已经实例化后的物体
            GameObject go = obj.Result;
            go.transform.position = Vector3.left * 2;
        };
    }

    #endregion

    #region 异步形式

    private async void AsyncLoadCube()
    {
        // 虽然这里使用了Task，但并没有使用多线程
        GameObject prefabObj = await Addressables.LoadAssetAsync<GameObject>("Cube").Task;
        // 实例化
        GameObject cubeObj = Instantiate(prefabObj);
        cubeObj.transform.position = Vector3.zero;
    }

    private async void AsyncInstantiateCube()
    {
        // 直接使用InstantiateAsync方法
        GameObject cubeObj = await Addressables.InstantiateAsync("Cube").Task;
        cubeObj.transform.position = Vector3.right * 2;
    }
    #endregion

    #region 面板形式

    private void RefLoadCube()
    {
        cubeRef.LoadAssetAsync<GameObject>().Completed += (obj) =>
        {
            // 加载完成结果
            GameObject cubePrefab = obj.Result;
            // 实例化
            GameObject cubeObj = Instantiate(cubePrefab);
            // 修改位置
            cubeObj.transform.position = Vector3.zero;
        };
    }
    #endregion

    #region 同步加载

    private void InstantiatePrefab()
    {
        // 实例化加载到的游戏物体
        Instantiate(LoadPrefab(), Vector3.zero, Quaternion.identity);
    }

    // 强制同步加载GameObject的基本用法
    private GameObject LoadPrefab()
    {
        var op = Addressables.LoadAssetAsync<GameObject>("Cube");
        GameObject go = op.WaitForCompletion();
        return go;
    }
    #endregion
}