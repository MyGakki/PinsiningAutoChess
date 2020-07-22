using System;
using UnityEngine;

public class NormalSingleton<T> where T: IGameBase, new()
{
    private static T instance;
    public static T Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new T();
                instance.Init();
            }

            return instance;
        }
    }
}

public class MonoBehaviourSingleton<T> : MonoBehaviour where T : MonoBehaviourSingleton<T>
{
    private static T instance;

    public static T Instance
    {
        get { return instance; }
    }

    public void Awake()
    {
        instance = this as T;
    }
}
