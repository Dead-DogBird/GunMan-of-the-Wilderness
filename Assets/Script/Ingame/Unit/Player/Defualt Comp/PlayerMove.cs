using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Random = UnityEngine.Random;

//플레이어의 활동(ex: 점프 좌우 움직임 등등)
//이후 충돌 처리 등등을 처리
public class PlayerMove : MonoBehaviour
{
    //컴포넌트들
    public Rigidbody2D _playerRigidbody { get; private set; }
    private PlayerContrl _playerControll;
    private PlayerState _playerState;
    //세팅값들
    [SerializeField] private float speed = 5f;
    [SerializeField] private float jumpForce = 1000;
    
    //기타 트리거들
    [SerializeField] private bool _isjumping;
    private int jumpCount = 0;
    
    
    private void Start()
    {
        
        _playerRigidbody = GetComponent<Rigidbody2D>();
        _playerControll = GetComponent<PlayerContrl>();
        _playerState = GetComponent<PlayerState>();
        WalkParticle().Forget();
    }

    private void Update()
    {
        Jump();
        UpdateAnimation();
        transform.Translate(new Vector3(speed*_playerControll.Userinput.AxisState, 0) * Time.deltaTime);
    }
    private void FixedUpdate()
    {
      
    }

    private void Jump()
    {
        if (jumpCount >= 2) return;
        if (!_playerControll.Userinput.SpaceState) return;
        _isjumping = true;
        jumpCount++;
        float radom = Random.Range(200f/255f, 255f/255f);
        GameManager.Instance.Effect(transform.position+new Vector3(-0.3f*_playerControll.Userinput.AxisState,0.2f),Random.Range(4,7),0.7f,
            new OrbitColors(new Color(radom,radom,radom,1),new Color(radom,radom,radom,1)),false,0,2);
        if (jumpCount == 2)
        {
            _playerState.PlayAnimation("Player@jump",-1,0);
            _playerRigidbody.velocity = Vector2.zero;
            _playerRigidbody.AddForce(new Vector2(0, jumpForce));
            return;
        }
        _playerRigidbody.AddForce(new Vector2(0, jumpForce));
        _playerRigidbody.velocity = Vector2.zero;
        _playerState.isGround = !_isjumping;
        
       
    }

    void UpdateAnimation()
    {
        _playerState.isGround = !_isjumping;
        _playerState.isMove = _playerControll.Userinput.AxisState != 0;
    }
    private void OnCollisionEnter2D(Collision2D other)
    {

    }

    private void OnCollisionStay2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Ground")&&other.contacts[1].normal.y>0.7f)
        {
            _isjumping = false;
            jumpCount = 0;
            if (_playerControll.Userinput.AxisState != 0)
            {
                
            }
        }
    }

    private void OnCollisionExit2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Ground"))
        {
            _isjumping = true;
            jumpCount = 1;
        }
    }

    async UniTaskVoid WalkParticle()
    {
        while (gameObject.activeSelf)
        {
            float radom = Random.Range(200f/255f, 255f/255f);
            GameManager.Instance.Effect(transform.position+new Vector3(-0.3f*_playerControll.Userinput.AxisState,0.2f),Random.Range(1,4),0.7f,
                new OrbitColors(new Color(radom,radom,radom,1),new Color(radom,radom,radom,1)),false,0,2);
            await UniTask.WaitUntil(() => _isjumping == false&&_playerState.isMove);
            await UniTask.Delay(TimeSpan.FromSeconds(0.1f),ignoreTimeScale:false);
        }
    }
}
