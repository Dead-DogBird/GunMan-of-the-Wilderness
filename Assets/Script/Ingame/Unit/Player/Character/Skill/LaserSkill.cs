using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Random = UnityEngine.Random;

public class LaserSkill : MonoBehaviour
{
    private BoxCollider2D _boxCollider;
    private bool isDestory= false;
    [SerializeField] private float speed = 35;
    [SerializeField] private Color[] _colors;
    // Start is called before the first frame update
    void Start()
    {
        _boxCollider = GetComponent<BoxCollider2D>();
        updateTask().Forget();
        Destroy(gameObject,2f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        transform.Translate(Vector3.right * (speed * Time.deltaTime));
    }

    async UniTaskVoid updateTask()
    {
        int num = 0;
        while (!isDestory)
        {
            num++;
            _boxCollider.enabled = (num % 3 == 0);
            float angle = Random.Range(0f, 360f); // 0부터 360도 사이의 랜덤한 각도
            float radians = angle * Mathf.Deg2Rad; // 각도를 라디안으로 변환
            float x = 3.25f * Mathf.Cos(radians); // x 좌표 계산
            float y = 3.25f * Mathf.Sin(radians);
            GameManager.Instance.MoveOrbitEffect(transform.position + new Vector3(x, y), Random.Range(3, 5), 0.9f,
                    new OrbitColors(_colors[Random.Range(0, 3)], _colors[Random.Range(0, 3)]),
                    false, 0, 1.6f, 0, Random.Range(7, 17), 180);
            GameManager.Instance.MoveOrbitEffect(transform.position + new Vector3(x, y), Random.Range(3, 5), 0.9f,
                new OrbitColors(_colors[Random.Range(0, 3)], _colors[Random.Range(0, 3)]),
                false, 0, 1.6f, 0, Random.Range(7, 17), 180);
            GameManager.Instance.MoveOrbitEffect(transform.position + new Vector3(x, y), Random.Range(3, 5), 0.9f,
                new OrbitColors(_colors[Random.Range(0, 3)], _colors[Random.Range(0, 3)]),
                false, 0, 1.6f, 0, Random.Range(7, 17), 180);
            await UniTask.Yield(PlayerLoopTiming.FixedUpdate);
        }
    }
    private void OnDestroy()
    {
        isDestory = true;
    }
}
