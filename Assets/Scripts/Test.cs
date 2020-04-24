using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class Test : MonoBehaviour
{
    public Image image;
    // Use this for initialization
    void Start()
    {
        image.sprite = FileIO.LoadSprite(R.SpritePack.HELPER_HELP_BDSD);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
