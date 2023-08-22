using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using Cysharp.Threading.Tasks;
using Unity.Mathematics;
using UnityEngine;
using Vector2 = UnityEngine.Vector2;

public class RevolverFire : PlayerFire
{
    [SerializeField] private GameObject missle;
    // Start is called before the first frame update
    void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    void Update()
    {
        base.Update();
    }

    protected override void Skill()
    {
        Missle().Forget();
    }
    async UniTaskVoid Missle()
    {
        Debug.Log("미사일 발싸");
        Collider2D[] colls = Physics2D.OverlapCircleAll(transform.position, 20.0f);
        Transform monster=null;
        float closestdistance = 20;
        float distance;
        foreach (var var_ in colls)
        {
            if (var_.transform.CompareTag("Monster"))
            {
                distance = Vector2.Distance( var_.transform.position, transform.position);
                if (distance < closestdistance)
                {
                    closestdistance = distance;
                    monster = var_.transform;
                }
            }
        }

        if (monster is not null) 
        {
            for (int i = 0; i < 5; i++)
            {
                _audioManager.PlaySFX(skillSfxId);
                Instantiate(missle).
                GetComponent<RevolverRoket>().Init(transform,monster, 
                    2f, 4, 4, 1.5f,_playerState.colors,_playerState.GetFireInstance().damage*2);
                await UniTask.Delay(TimeSpan.FromSeconds(0.15f), ignoreTimeScale: false);
            }
        }
        
    }
}
