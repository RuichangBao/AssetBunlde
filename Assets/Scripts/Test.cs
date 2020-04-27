using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class Test : MonoBehaviour
{
    public RawImage image;
    // Use this for initialization
    void Start()
    {
        image.texture = FileIO.LoadTexture2D(R.Texture.TUPIAN_ALI);
    }
}