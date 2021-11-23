using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 单例类
/// </summary>
/// <typeparam name="T"></typeparam>
public class Single<T> where T : new()
{
    private static T instance;

    public static T Instance
    {
        get 
        {
            if(instance==null)
            {
                instance = new T();
                (instance as Single<T>).Init();
            }
            return instance;
        }
    }
    public virtual void Init()
    {

    }

}
