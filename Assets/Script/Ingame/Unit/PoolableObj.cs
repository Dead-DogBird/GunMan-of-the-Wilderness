using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading;
using Cysharp.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;

public class PoolableObj : MonoBehaviour
{
   
    // Start is called before the first frame update
    protected void Start()
    {
       
        ReleseReserv().Forget();
    }

    protected void OnEnable()
    {
      
        ReleseReserv().Forget();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    protected void OnDisable()
    {
    
    }

    public virtual GameObject Init()
    {
        return gameObject;
    }

    protected virtual async UniTaskVoid ReleseReserv(float delay = 2)
    {
        
            await UniTask.Delay(TimeSpan.FromSeconds(delay), ignoreTimeScale: false);
            if (gameObject.activeSelf)
                GameManager.Instance._poolingManager.Despawn(this);
      
    }
}
