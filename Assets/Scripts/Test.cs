using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Test : MonoBehaviour
{
    public Image image;
    // Use this for initialization
    void Start()
    {
        image.sprite = FileIO.LoadSprite(R.SpritePack.BUILD_BUBINGYING);

    }

}