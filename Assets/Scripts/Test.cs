using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class Test : MonoBehaviour
{
    public GameObject obj1;
    public GameObject obj2;
    public Vector3 vector1;
    public Vector3 vector2;

    // Use this for initialization
    void Start()
    {
        vector1 = new Vector3(1, 1, 1);
        vector2 = new Vector3(1, 1, 1);
    }

    // Update is called once per frame
    void Update()
    {
        Debug.LogError(test(obj1, obj2, vector1, vector2));
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="obj1">物体1</param>
    /// <param name="obj2">物体2</param>
    /// <param name="vector1">物体1的长宽高</param>
    /// <param name="vector2">物体2的长宽高</param>
    /// <returns></returns>
    public bool test(GameObject obj1,GameObject obj2,Vector3 vector1,Vector3 vector2)
    {
        Vector3 pos1 = obj1.transform.position;
        Vector3 pos2 = obj2.transform.position;
        Vector3 vectorPos = (obj2.transform.position - obj1.transform.position);//方向向量
        Debug.LogError("方向向量:"+vectorPos);
        //方向向量与物体1的交点
        Vector3 vectorPos1 = new Vector3(vectorPos.x / vector1.x / 2, vectorPos.y / vector1.y / 2, vectorPos.z / vector1.z / 2);
        //方向向量与物体2的交点
        Vector3 vectorPos2 = vectorPos - vector2 / 2;
            // new Vector3(vectorPos.x - vector2.x / 2, vectorPos.y - vector2.y / 2, vectorPos.z - vector2.z / 2);
       
        //vectorPos + obj2 .transform.position- new Vector3( vectorPos.x / vector2.x /2, vectorPos.y / vector2.y / 2, vectorPos.z / vector2.z / 2);
        if ((vectorPos1-obj1.transform.position).magnitude + (vectorPos2 - obj2.transform.position).magnitude >= vectorPos.magnitude)
        {
            return true;
        }
        return false;
    }
}
