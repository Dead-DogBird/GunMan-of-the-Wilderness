using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//플레이어의 활동(ex: 점프 좌우 움직임 등등)
//이후 충돌 처리 등등을 처리
public class PlayerMove : MonoBehaviour
{
    //컴포넌트들
    private Rigidbody2D _playerRigidbody;
    private PlayerContrl _playerControll;

    //세팅값들
    [SerializeField] private float speed = 5f;
    [SerializeField] private float jumpForce = 1000;
    
    //기타 트리거들
    [SerializeField] private bool _isjumping;

    private void Start()
    {
        _playerRigidbody = GetComponent<Rigidbody2D>();
        _playerControll = GetComponent<PlayerContrl>();
    }

    private void Update()
    {
        Jump();
    }
    private void FixedUpdate()
    {
        Debug.DrawRay(_playerRigidbody.position, Vector3.down, new Color(0,1,0)); 
        transform.Translate(new Vector3(speed*_playerControll.Userinput.AxisState, 0) * Time.deltaTime);
       
        RaycastHit2D raycastHit2D = Physics2D.Raycast(transform.position, Vector3.down
            , 0.5f, LayerMask.GetMask("Platform"));
        if (_playerRigidbody.velocity.y <= 0)
        {
            if (raycastHit2D.collider != null)
            {
                if (raycastHit2D.distance < 0.2f)
                {
                    _isjumping = false;
                }
            }
        }
    }

    private void Jump()
    {
        if (!_playerControll.Userinput.SpaceState) return;
        if (_isjumping)
            return;
        _isjumping = true;
        _playerRigidbody.AddForce(new Vector2(0, jumpForce));
        _playerRigidbody.velocity = Vector2.zero;
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        /*if (other.gameObject.CompareTag("Ground")&&other.transform.position.y<transform.position.y)
        {
            _isjumping = false;
        }*/
    }
}
