using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

public class DMRTurret : MonoBehaviour
{
    private PlayerState player;
    private DMRFire _owner;
    [SerializeField] private GameObject _bullet;
    [SerializeField] private GameObject _thunder;
    [SerializeField] private Transform firePos;
    private bool isActive = true;
    
    // Start is called before the first frame update
    void Start()
    {
        Destroy(Instantiate(_thunder, transform.position, Quaternion.identity),0.2f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnDestroy()
    {
        isActive = false;
        _owner.isTurretActive = false;
    }

    public void Init(PlayerState _player,DMRFire owner)
    {
        player = _player;
        _owner = owner;
        _owner.isTurretActive = true;
        Fire().Forget();
    }
    async UniTaskVoid Fire()
    {
        while (isActive)
        {
            TurretFire();
            await UniTask.Delay(TimeSpan.FromSeconds(0.5f));
        }
    }
    void TurretFire()
    {
        Destroy(Instantiate(_thunder, transform.position, Quaternion.identity),0.2f);
        Collider2D[] colls = Physics2D.OverlapCircleAll(transform.position, 30f);
        float closestdistance = 20;
        float distance;
        Vector3 toVector = default;
        foreach (var var_ in colls)
        {
            if (var_.transform.CompareTag("Monster"))
            {
                distance = Vector2.Distance(var_.transform.position, transform.position);
                if (distance < closestdistance)
                {
                    closestdistance = distance;
                    toVector = var_.transform.position;
                    
                }
            }
        }
        if(toVector != default)
        {
            Instantiate(_bullet).GetComponent<turretBullet>().Init(
            new GetFireInstance(firePos.position,firePos.position,toVector,player.getDamage,player.getBulletSpeed,Color.black,
                player.colors));
        }
        
    }
    
    
}
