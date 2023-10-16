using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

public class GatlingFire : PlayerFire
{
    private int count;
    [SerializeField] private Transform[] firePoses;
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
       
    }

    protected override void Fire()
    {
        count++;
        var instance = _playerState.GetFireInstance();
        instance.firepos = firePoses[Random.Range(0, firePoses.Length)].position;
        if(count%4==0)
        {      
            UIManager.Instance.SetCursorEffect();
            Instantiate(_fireFlame, instance.firepos,Quaternion.identity).
                transform.localEulerAngles = new Vector3(0,0,_playerState._playerGun.rotateDegree);
        }
        _audioManager.PlaySFX(fireSfxId);
        GameManager.Instance._poolingManager.Spawn<Bullet>().Init(instance,0.13f);
        GameManager.Instance.MoveOrbitEffect(instance.firepos,Random.Range(3,5),0.7f,
            _playerState.colors,
            false,0,2, (_playerState._playerGun.rotateDegree+180),Random.Range(7,12),30);
    }
    /* new void Start()
    {
        _playerState = GetComponent<PlayerState>();
        _myDamage = _playerState.getDamage;
        _audioManager = AudioManager.Instance;
    }
    private bool isFire = false;
    
    new void Update()
    {
        if (_playerState._playerContrl.Userinput.LeftMouseState)
        {
            _audioManager.PlaySFX(fireSfxId);
            FireTask().Forget();
        }
        if (_playerState.GetSkill())
        {
            Skill();
        }
        if (_playerState.GetReload())
        {
            _playerState.ReLoad().Forget();
        }
        
    }
    protected override void Fire()
    {
        
    }

    private bool isTaskRunning = false;
    async UniTaskVoid FireTask()
    {
        if (isTaskRunning)
        {
            
            return;
        }
        isTaskRunning = true;
        await UniTask.Delay(TimeSpan.FromSeconds(0.5f));
        _GunFire.SetActive(true);
        var FireColider = _GunFire.GetComponent<Collider2D>();
        int audioSourceIndex = _audioManager.PlaySFX(fireSfxId,true);
        int count =0;
        while (_playerState._playerContrl.Userinput.LeftMouseState&&_playerState.FireState!=PlayerState.FireState_.NoMag)
        {
            Debug.Log("빵야빵야");
            count++;
            FireColider.enabled = count % 2 == 0;
            if(count % 3 == 0)
                Instantiate(_fireFlame, _playerState.GetFireInstance().firepos,Quaternion.identity);
            GameManager.Instance.MoveOrbitEffect(_playerState.GetGunholePos.TransformPoint((_playerState.GetGunholePos.localPosition+
                    new Vector3(0,Random.Range(-0.25f,0.25f),0))),1,Random.Range(0.60f,1.30f),_playerState.GetFireInstance().orbitcolors,true,0.4f,2f,
                _playerState._playerGun.rotateDegree,Random.Range(10,26),3);
            await UniTask.WaitUntil(()=>_playerState.GetFire());
        }
        _audioManager.StopLoopedSfx(audioSourceIndex);
        await UniTask.Delay(TimeSpan.FromSeconds(0.3f));
        isTaskRunning = false;
        _GunFire.SetActive(false);
    }*/
}
