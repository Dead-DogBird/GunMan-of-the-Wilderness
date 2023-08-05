using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using JetBrains.Annotations;


// 플레이어의 체력, 기술 게이지, 탄약 등등의 총체적인 상황을 관리
// 충돌 등등에도 관여.
// ReSharper disable once CheckNamespace
public class PlayerState : MonoBehaviour
{
    //체력
    [SerializeField] private float _playerHp = 100;

    //탄창
    [SerializeField] private int _allMag;
    [SerializeField] private int _nowMag;
  
    [SerializeField] private float _skillgauge = 100;
    [SerializeField] private float _nowskillgauge;

    [SerializeField] private int _money;
    [SerializeField] private float _damage;

    [SerializeField] private float _fireDelay;
    private float _nowFireDelay;

    [SerializeField] private float _reloadDelay;
    [SerializeField] private float _bulletSpeed = 1;
    
    
    private FireState_ _fireState;

    public float getPlayerHp => _playerHp;
    public int getAllMag => _allMag;
    public float getSkillgauge => _skillgauge;
    public float getNowSkillgauge => _nowskillgauge;
    public int getMoney => _money;
    public int getNowMag => _nowMag;
    public float getDamage => _damage;
    public float getBulletSpeed => _bulletSpeed;
    
    private Rigidbody2D _playerRigidbody;
    private PlayerContrl _playerContrl;

    [SerializeField] private GameObject Textures;
    private List<Sprite> _charSprites = new();


    private enum FireState_
    {
        Default,
        Reload,
        Delayed,
        NoMag
    };

    private void Start()
    {
        _playerRigidbody = GetComponent<Rigidbody2D>();
        _playerContrl = GetComponent<PlayerContrl>();
        _fireState = FireState_.Default;
        Getsprite(Textures);
    }

    private void Getsprite(GameObject obj)
    {
        Debug.Log(obj.transform.childCount);
        for (var i = 0; i < obj.transform.childCount; i++)
        {
            var child = obj.transform.GetChild(i);
            _charSprites.Add(child.GetComponent<SpriteRenderer>().sprite);
        }
        
    }
    private void Update()
    {

    }

    public bool GetSkill()
    {
        if (!_playerContrl.Userinput.SkillKey||
            !(_nowskillgauge >= _skillgauge)) return false;
        
        _nowskillgauge = 0;
        return true;
    }

    private FireState_ GetFireCommand()
    {
        switch (_fireState)
        {
            case FireState_.Reload:
                return _fireState;
            case FireState_.Default when _nowMag <= 0:
                _fireState = FireState_.NoMag;
                ReLoad().Forget();
                return _fireState;
            case FireState_.Default:
                _nowMag--;
                Fired().Forget();
                return FireState_.Default;
            default:
                return _fireState;
        }
    }
    public bool GetFire()
    {
        return _playerContrl.Userinput.LeftMouseState&&
               (GetFireCommand() == FireState_.Default);
    }

    public Vector3 GetMousePos()
    {
        return _playerContrl.Userinput.MousePos;
    }
    private async UniTaskVoid Fired()
    {
        _fireState = FireState_.Delayed;
        await UniTask.Delay(TimeSpan.FromSeconds(_fireDelay), ignoreTimeScale: false);
        _fireState = FireState_.Default;
    }

    private async UniTaskVoid ReLoad()
    {
        _fireState = FireState_.Reload;
        await UniTask.Delay(TimeSpan.FromSeconds(_reloadDelay), ignoreTimeScale: false);
        _nowMag = _allMag;
        _fireState = FireState_.Default;
    }

    public void GetDamage(float getDamage)
    {
        
    }

    async UniTaskVoid GetDamegeTask()
    {
        
        
    }
    private void OnCollisionEnter2D(Collision2D other)
    {
        
    }
}
