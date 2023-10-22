using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GatlingSkill : MonoBehaviour
{
    private int num =0;

    private CircleCollider2D _collider;
    // Start is called before the first frame update
    void Start()
    {
        _collider = GetComponent<CircleCollider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        num++;
        _collider.enabled = (num % 3 == 0);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("MonsterBullet"))
        {
            GameManager.Instance._poolingManager.Despawn(other.GetComponent<MonsterBullet>());
        }
    }
}
