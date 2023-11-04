using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;

public class LoadByLabelManager : MonoBehaviour
{
    // 可寻址系统标签的引用
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
    /// 根据标签加载
    /// </summary>
    private void LoadGameObjectByLabel()
    {
        Addressables.LoadAssetsAsync<Texture2D>(prefabsLabel, (texture) =>
        {
            // 每加载完成一个，就回调一次。 标签下有几个就会执行几次
            Debug.Log("加载完成一个资源： " + texture.name);
            _rawImage.texture = texture;
            _rawImage.SetNativeSize();
        });
    }

    /// <summary>
    /// 根据地址和标签加载
    /// </summary>
    /// <param name="key">可寻址资源地址</param>
    /// <param name="label">资源标签</param>
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
