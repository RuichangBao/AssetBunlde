using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Test : MonoBehaviour
{
    public GameObject obj;
    void Start()
    {
        string str = obj.GetComponent<MeshRenderer>().name;
        Debug.LogError(str);
        str = obj.GetComponent<MeshFilter>().name;
        Debug.LogError(str);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
