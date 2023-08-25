using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;

public class DMRTurret : MonoBehaviour
{
    private PlayerState player;
    private DMRFire _owner;
    [SerializeField] private GameObject _attackObject;
    [SerializeField] private GameObject _thunder;
    private bool isActive = true;
    [SerializeField] private int sfxId = 12;
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
            AudioManager.Instance.PlaySFX(sfxId);
            TurretFire();
            await UniTask.Delay(TimeSpan.FromSeconds(0.5f));
        }
    }
    void TurretFire()
    {
        IngameCamera.Instance.Shake(0f,0.2f,0,1,10f);
        Destroy(Instantiate(_thunder, transform.position, Quaternion.identity),0.2f);
        Collider2D[] colls = Physics2D.OverlapCircleAll(transform.position, 10f);
        foreach (var var_ in colls)
        {
            if (var_.transform.CompareTag("Monster")||var_.transform.CompareTag("BossMonster"))
            {
                Instantiate(_attackObject,var_.transform.position,quaternion.identity);
            }
        }
    }
}
