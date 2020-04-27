using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class Test : MonoBehaviour
{
    public AudioSource audioSource;
    // Use this for initialization
    void Start()
    {
        audioSource.clip = FileIO.LoadAudioClip(R.AudioClip.TESTAUDIOCLIP_LOGINBGM);
        audioSource.Play();
    }
}