using System;
using System.Collections;
using System.Collections.Generic;
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

    private void OnEnable()
    {
        ReleseReserv().Forget();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public virtual GameObject Init()
    {
        return gameObject;
    }

    protected virtual async UniTaskVoid ReleseReserv(float delay = 2)
    {
        await UniTask.Delay(TimeSpan.FromSeconds(delay), ignoreTimeScale: false);
        GameManager.Instance._poolingManager.Despawn(this);
    }
}
