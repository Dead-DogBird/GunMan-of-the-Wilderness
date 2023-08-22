using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonoSingleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T _instance;
    public static T Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<T>();
                if (_instance == null)
                    return null;
            }

            
            return _instance;
        }
    }

    protected void Awake()
    {
        if (_instance == null)
        {
            _instance = this as T;
        }
        else
        {
            Debug.Log($"{typeof(T)}이미 있대서 삭제함 ㅅㄱ");
            Debug.Log(_instance.name);
            Debug.Log(gameObject.name);
            Destroy(gameObject);
        }
    }
}