using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using JetBrains.Annotations;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

public class RevolverRoket : MonoBehaviour
{
    private Vector3[] m_points = new Vector3[4];
    private float m_timerMax = 0;
    private float m_timerCurrent = 0;
    private float m_speed;
    Vector3 _endP;
    Transform _endT_;
    private OrbitColors _colors;
    public float Damege { get; private set; }
    public GameObject LockOn;
    private GameObject targeted;
    [SerializeField] private int sfxId = 19;
    public void Init(Transform _startTr, Transform _endTr, float _speed, float _newPointDistanceFromStartTr,
        float _newPointDistanceFromEndTr, float _timer, OrbitColors colors, float damege)
    {
        _colors = colors;
        m_speed = _speed;
        Damege = damege;
        // 끝에 도착할 시간을 랜덤으로 줌.
        m_timerMax = _timer;
        // 시작 지점.
        m_points[0] = _startTr.position;
        // 시작 지점을 기준으로 랜덤 포인트 지정.
        m_points[1] = _startTr.position +
                      (_newPointDistanceFromStartTr * Random.Range(-0.5f, 0.5f) * _startTr.right) + // X (좌, 우 전체)
                      (_newPointDistanceFromStartTr * Random.Range(-2f, 2.0f) * _startTr.up); // Y (아래쪽 조금, 위쪽 전체)
        if(!_endTr)
            Reserching();
        else
        {
            // 도착 지점을 기준으로 랜덤 포인트 지정.
            m_points[2] = _endTr.position +
                          (_newPointDistanceFromEndTr * Random.Range(-0.5f, 0.5f) * _endTr.right) + // X (좌, 우 전체)
                          (_newPointDistanceFromEndTr * Random.Range(-2.0f, 2.0f) * _endTr.up);
            // 도착 지점.
            m_points[3] = _endTr.position;
            _endP = -_endTr.position;
            _endT_ = _endTr;
        }

        try
        {
            if (!CheckForChild(_endT_.gameObject, "LockOn(Clone)"))
                targeted = Instantiate(LockOn, _endT_);
            transform.position = _startTr.position;
            MakeOrbit().Forget();
            Destroy(gameObject, 2f);
        }
        catch (NullReferenceException e)
        {
            Explosion();
        }
    }

    void Update()
    {
        if (_endT_ == null)
            Reserching();
        if (m_timerCurrent > m_timerMax)
        {
            return;
        }

        m_timerCurrent += Time.deltaTime * m_speed;

        if (_endT_ != null)
        {
            Vector3 to_t = new Vector3(
                CubicBezierCurve(m_points[0].x, m_points[1].x, m_points[2].x, _endT_.position.x),
                CubicBezierCurve(m_points[0].y, m_points[1].y, m_points[2].y, _endT_.position.y),
                CubicBezierCurve(m_points[0].z, m_points[1].z, m_points[2].z, _endT_.position.z)
            );
            transform.position = to_t;
            transform.rotation =
                Quaternion.Euler(0, 0, CustomAngle.PointDirection(transform.position, _endT_.position));
        }
    }
    private float CubicBezierCurve(float a, float b, float c, float d)
    {
        var t = m_timerCurrent / m_timerMax;

        var ab = Mathf.Lerp(a, b, t);
        var bc = Mathf.Lerp(b, c, t);
        var cd = Mathf.Lerp(c, d, t);

        var abbc = Mathf.Lerp(ab, bc, t);
        var bccd = Mathf.Lerp(bc, cd, t);

        return Mathf.Lerp(abbc, bccd, t);
    }

    private void OnDestroy()
    {
        if(targeted)
            Destroy(targeted);
    }

    private float random;
    public float OrbitDelay = 0.004f;

    async UniTaskVoid MakeOrbit()
    {
        while (true)
        {
            if (this.IsUnityNull())
                throw new OperationCanceledException();

            GameManager.Instance._poolingManager.Spawn<Orbit>().Init(new OrbitInfo(true, transform.position
                + new Vector3(Random.Range(0.15f, -0.15f), Random.Range(0.15f, -0.15f)),
                transform.localScale.x + Random.Range(-0.2f, 0.02f), 0.2f, _colors), 0.45f);
            await UniTask.Delay(TimeSpan.FromSeconds(OrbitDelay), ignoreTimeScale: false);
        }
    }

    void Reserching()
    {
        Collider2D[] colls = Physics2D.OverlapCircleAll(transform.position, 30f);
        float closestdistance = 20;
        float distance;
        foreach (var var_ in colls)
        {
            if (var_.transform.CompareTag("Monster")||var_.transform.CompareTag("BossMonster"))
            {
                distance = Vector2.Distance(var_.transform.position, transform.position);
                if (distance < closestdistance)
                {
                    closestdistance = distance;
                    _endT_ = var_.transform;
                }
            }
        }

        try
        {
            m_points[0] = transform.position;
            m_points[1] = transform.position +
                          (5 * Random.Range(-0.5f, 0.5f) * transform.right) +
                          (5 * Random.Range(-2f, 2.0f) * transform.up);
            m_points[2] = _endT_.position +
                          (5 * Random.Range(-0.5f, 0.5f) * _endT_.right) +
                          (5 * Random.Range(-0.5f, 0.5f) * _endT_.up);
            m_points[3] = _endT_.position;
            m_timerCurrent = 0;
            _endP = -_endT_.position;
            if (!CheckForChild(_endT_.gameObject, "LockOn(Clone)"))
                targeted = Instantiate(LockOn, _endT_);
        }
        catch (MissingReferenceException e)
        {
            Explosion();
        }
        catch (NullReferenceException e)
        {
            Explosion();
        }
    }

    public void Explosion()
    {
        AudioManager.Instance.PlaySFX(sfxId);
        Collider2D[] colls = Physics2D.OverlapCircleAll(transform.position, 3.0f);
        foreach (var var_ in colls)
        {
            if (var_.transform.CompareTag("Monster"))
            {
                var_.GetComponent<MonsterDefault>().OnDameged(new DamageInfo(Damege,_colors.priColor,transform.position));
            }

            if (var_.transform.CompareTag("BossMonster"))
            {
                {
                    var_.GetComponent<DefaultBossMonster>().OnDameged(new DamageInfo(Damege,_colors.priColor,transform.position));
                }
            }
        }
        for (int i = 0; i < Random.Range(3, 6); i++)
        {
            GameManager.Instance.Effect(transform.position,
                1, Random.Range(1f, 2f), _colors);
        }
        IngameCamera.Instance.Shake(0.05f,0.05f,0,1.03f,10f);
        Destroy(gameObject);
    }
    private bool CheckForChild(GameObject parentObject, string childObjectName)
    {
        Transform childTransform = parentObject.transform.Find(childObjectName);

        if (childTransform != null)
        {
            return true;
        }

        return false;
    }
}
