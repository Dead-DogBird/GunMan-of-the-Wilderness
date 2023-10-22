using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class LaserFire : PlayerFire
{
    [SerializeField] private GameObject _laser;
    [SerializeField] private BoxCollider2D _laserCollider;
    [SerializeField] private GameObject _laserSkill;
    private LaserGunControll _controll;
    // Start is called before the first frame update
    void Start()
    {
        _playerState = GetComponent<PlayerState>();
        _myDamage = _playerState.getDamage;
        _audioManager = AudioManager.Instance;
        _controll = _laser.transform.GetComponent<LaserGunControll>();
        _laser.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        base.Update();
    }
    protected override void Fire()
    {
        FireTask().Forget();
    }

    async UniTaskVoid FireTask()
    {
        _audioManager.PlaySFX(fireSfxId);
        UIManager.Instance.SetCursorEffect();
        _playerState.GetFireEffect();
        _laser.SetActive(true);
        var flame = Instantiate(_fireFlame, _playerState.GetFireInstance().firepos, Quaternion.identity);
        flame.transform.localEulerAngles = new Vector3(0,0,_playerState._playerGun.rotateDegree);
        flame.transform.parent = _laser.transform;
        flame.transform.localScale *= 2f;
        int i = 0;
        while(!_controll.isDone)
        {
            i++;
            _laserCollider.enabled = (i % 2 == 0);
            float angle = Random.Range(0f, 360f); // 0부터 360도 사이의 랜덤한 각도
            float radians = angle * Mathf.Deg2Rad; // 각도를 라디안으로 변환
            float x = 1.25f * Mathf.Cos(radians); // x 좌표 계산
            float y = 1.25f * Mathf.Sin(radians);
            GameManager.Instance.MoveOrbitEffect(_playerState.GetFireInstance().firepos+new Vector3(x,y),Random.Range(3,5),0.7f,
                _playerState.colors,
                false,0,2, (_playerState._playerGun.rotateDegree),Random.Range(18,30),0);
            await UniTask.Yield(PlayerLoopTiming.LastFixedUpdate);
        }

        _controll.isDone = false;
        _laser.SetActive(false);
        
    }

    protected override void Skill()
    {
        _audioManager.PlaySFX(skillSfxId);
        Instantiate(_laserSkill,transform.position + new Vector3(-45, 0, 0), Quaternion.identity);
    }
}
