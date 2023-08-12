using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Random = UnityEngine.Random;

public class MonsterDefault : MonoBehaviour
{
    [SerializeField] private float _hp = 100;

    [SerializeField] private float _speed = 3.5f;

    private Rigidbody2D _rigid;
    private bool _targetedPlayer = false;
    public int nextMove;

    private bool isDie = false;
    // Start is called before the first frame update
    void Start()
    {
        _rigid = GetComponent<Rigidbody2D>();
        MoveSelect().Forget();
    }

    // Update is called once per frame
    void Update()
    {
        if (!_targetedPlayer)
        {
            NontargetPlayerMove();
        }
    }

    private void OnDestroy()
    {
        isDie = true;
    }

    void NontargetPlayerMove()
    {
        _rigid.velocity = new Vector2(nextMove, _rigid.velocity.y);//왼쪽으로 가니까 -1, y축은 0을 넣으면 큰일남!
        //플랫폼 체크 
        //몬스터는 앞을 체크해야 
        Vector2 frontVec = new Vector2(_rigid.position.x + nextMove*0.2f,_rigid.position.y);
        RaycastHit2D rayHit = Physics2D.Raycast(frontVec, Vector3.down, 1, LayerMask.GetMask("Platform"));
        if (rayHit.collider==null)
        {
            nextMove *= -1;
            transform.localScale = nextMove > 0 ? new Vector3(-1,1) : new Vector3(1,1);
        }
    }
    
    async UniTaskVoid MoveSelect()
    {
        while (!isDie&&!_targetedPlayer)
        {
            nextMove = Random.Range(-1, 2);
            if(nextMove == 0)
                continue;
            
            transform.localScale = nextMove > 0 ? new Vector3(-1,1) : new Vector3(1,1);
            await UniTask.Delay(TimeSpan.FromSeconds(5), ignoreTimeScale: false);
        }
    }
}
