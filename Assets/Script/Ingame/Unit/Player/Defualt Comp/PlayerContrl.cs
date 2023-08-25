using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 플레이어의 조작을 입력받고 실시간으로 다른 컨트롤러에게 전달
public class PlayerContrl : MonoBehaviour
{
    public struct UserInput
    {
        float _nowAxisState;
        bool _nowSpace;
        bool _leftMouseButton;
        private Vector3 _mousePos;
        private bool _skillKey;
        bool _rkey;
        public bool SpaceState => _nowSpace;
        public bool LeftMouseState => _leftMouseButton;
        public float AxisState => _nowAxisState;
        public bool SkillKey => _skillKey;
        public Vector3 MousePos => _mousePos;
        public bool Rkey => _rkey;
        public void InputUpdate()
        {
            _nowAxisState = Input.GetAxisRaw("Horizontal");
            _nowSpace = Input.GetKeyDown(KeyCode.Space);
            _leftMouseButton = Input.GetMouseButton(0);
            _skillKey = Input.GetKeyDown(KeyCode.LeftShift);
            _rkey = Input.GetKey(KeyCode.R);
            _mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        }
    }
    public UserInput Userinput;
    void Start()
    {
        
    }
    void Update()
    {
        Userinput.InputUpdate();
    }
}
