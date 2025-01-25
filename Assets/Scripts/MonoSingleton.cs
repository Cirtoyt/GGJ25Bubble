using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MonoSingleton<T> : MonoBehaviour where T : MonoSingleton<T>
{
    public static T Instance
    {
        get
        {
            if (instance == null)
                Debug.LogError($"No instance for {typeof(T).ToString()} found! Maybe due to calling this from an Awake function?");

            return instance;
        }
    }

    private static T instance;

    protected virtual void Awake()
    {
        if (instance != null && instance != this)
            Destroy(this.gameObject);
        else
            instance = (T)this;
    }
}
