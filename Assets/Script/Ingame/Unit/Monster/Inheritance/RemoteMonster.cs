using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RemoteMonster : MonsterDefault
{
    new void Start()
    {
        base.Start();
    }
    new void OnEnable()
    {
     base.OnEnable();
    }
    new void Update()
    {
        base.Update();
    }
    new void OnDisable()
    {
        base.OnDisable();
    }
    new void OnDestroy()
    {
        base.OnDestroy();
    }

    protected override void TargetedPlayer()
    {
        if (Mathf.Abs(transform.position.x - player.transform.position.x) >= distanceFromPlayer)
        {
            transform.Translate(new Vector3(_speed*(player.transform.position.x > transform.position.x? 2 : -2), 0) * Time.deltaTime);
        }
        else
        {
            transform.Translate(new Vector3(_speed*(player.transform.position.x > transform.position.x? -1.5f : 1.5f), 0) * Time.deltaTime);
        }
        if (player.transform.position.x < transform.position.x&&transform.localScale.x<0)
        {
            transform.localScale = new Vector3(1, 1, 1);
        }
        else if (player.transform.position.x > transform.position.x&&transform.localScale.x>0)
        {
            transform.localScale = new Vector3(-1, 1, 1);
        }   
        var frontVec = new Vector2(_rigid.position.x + (transform.localScale.x>0 ? -0.5f:0.5f),_rigid.position.y+0.2f);
        Debug.DrawLine(frontVec,frontVec+new Vector2(0,1),Color.red);
        if (Physics2D.Raycast(frontVec, Vector3.up, 1, LayerMask.GetMask("Platform")))
        {
            _rigid.AddForce(new Vector2(0,1),ForceMode2D.Impulse);
        }
    }
}
