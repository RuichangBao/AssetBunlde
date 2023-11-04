using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;

public class LoadByLabelManager : MonoBehaviour
{
    // ��Ѱַϵͳ��ǩ������
    public AssetLabelReference prefabsLabel;

    public Button loadWhiteLogoBtn;

    public RawImage _rawImage;

    private void Start()
    {
        loadWhiteLogoBtn.onClick.AddListener(() =>
        {
            LoadTextureByKeyLabel("Logo", "Black");
        });

        LoadGameObjectByLabel();
    }

    /// <summary>
    /// ���ݱ�ǩ����
    /// </summary>
    private void LoadGameObjectByLabel()
    {
        Addressables.LoadAssetsAsync<Texture2D>(prefabsLabel, (texture) =>
        {
            // ÿ�������һ�����ͻص�һ�Ρ� ��ǩ���м����ͻ�ִ�м���
            Debug.Log("�������һ����Դ�� " + texture.name);
            _rawImage.texture = texture;
            _rawImage.SetNativeSize();
        });
    }

    /// <summary>
    /// ���ݵ�ַ�ͱ�ǩ����
    /// </summary>
    /// <param name="key">��Ѱַ��Դ��ַ</param>
    /// <param name="label">��Դ��ǩ</param>
    private void LoadTextureByKeyLabel(string key, string label)
    {
        Addressables.LoadAssetsAsync<Texture2D>(new List<string> { key, label },
            null, Addressables.MergeMode.Intersection).Completed += TextureLoaded;
    }

    private void TextureLoaded(AsyncOperationHandle<IList<Texture2D>> texture)
    {
        _rawImage.texture = texture.Result[0];
        _rawImage.SetNativeSize();
    }
}
