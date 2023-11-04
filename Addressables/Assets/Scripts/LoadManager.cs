using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class LoadManager : MonoBehaviour
{  
    // ��Ѱַ��Դ��������
    public AssetReference cubeRef;
    private void Start()
    {
        // �ص���ʽ
        //LoadGameObject();
        //LoadGameObjectCallBack();
        //InstantiateGameObject();

        // �첽��ʽ
        //AsyncLoadCube();
        //AsyncInstantiateCube();

        //this.RefLoadCube();

        Addressables.InitializeAsync();
        // ͬ������
        InstantiatePrefab();
    }

    #region �ص���ʽ

    /// <summary>
    /// �������� 
    /// �߼����Ҳ��踴�ã�ֱ��ʹ��Lambda���ʽ����ʽ
    /// </summary>
    private void LoadGameObject()
    {
        // ������"Cube" Ϊ��Ѱַϵͳ�ĵ�ַ
        Addressables.LoadAssetAsync<GameObject>("Cube").Completed += (obj) =>
        {
            GameObject go = obj.Result;

            Instantiate(go, Vector3.zero, Quaternion.identity);
        };
    }

    /// <summary>
    /// ��������
    /// </summary>
    private void LoadGameObjectCallBack()
    {
        Addressables.LoadAssetAsync<GameObject>("Assets/Prefabs/Sphere.prefab").Completed += LoadCallBack;
    }

    /// <summary>
    /// ��������Ļص�����
    /// </summary>
    private void LoadCallBack(AsyncOperationHandle<GameObject> handle)
    {
        GameObject go = handle.Result;
        Instantiate(go, Vector3.right * 2, Quaternion.identity);
    }

    /// <summary>
    /// ���ز�ʵ��������
    /// </summary>
    private void InstantiateGameObject()
    {
        Addressables.InstantiateAsync("Assets/Prefabs/Capsule.prefab").Completed += (obj) =>
        {
            // �Ѿ�ʵ�����������
            GameObject go = obj.Result;
            go.transform.position = Vector3.left * 2;
        };
    }

    #endregion

    #region �첽��ʽ

    private async void AsyncLoadCube()
    {
        // ��Ȼ����ʹ����Task������û��ʹ�ö��߳�
        GameObject prefabObj = await Addressables.LoadAssetAsync<GameObject>("Cube").Task;
        // ʵ����
        GameObject cubeObj = Instantiate(prefabObj);
        cubeObj.transform.position = Vector3.zero;
    }

    private async void AsyncInstantiateCube()
    {
        // ֱ��ʹ��InstantiateAsync����
        GameObject cubeObj = await Addressables.InstantiateAsync("Cube").Task;
        cubeObj.transform.position = Vector3.right * 2;
    }
    #endregion

    #region �����ʽ

    private void RefLoadCube()
    {
        cubeRef.LoadAssetAsync<GameObject>().Completed += (obj) =>
        {
            // ������ɽ��
            GameObject cubePrefab = obj.Result;
            // ʵ����
            GameObject cubeObj = Instantiate(cubePrefab);
            // �޸�λ��
            cubeObj.transform.position = Vector3.zero;
        };
    }
    #endregion

    #region ͬ������

    private void InstantiatePrefab()
    {
        // ʵ�������ص�����Ϸ����
        Instantiate(LoadPrefab(), Vector3.zero, Quaternion.identity);
    }

    // ǿ��ͬ������GameObject�Ļ����÷�
    private GameObject LoadPrefab()
    {
        var op = Addressables.LoadAssetAsync<GameObject>("Cube");
        GameObject go = op.WaitForCompletion();
        return go;
    }
    #endregion
}