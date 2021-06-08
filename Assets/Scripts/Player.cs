using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("Set in Inspector : Player")]   
    public Joystick joystick;

    [SerializeField] private float _speed = 10f;
    [SerializeField] private LayerMask _groundLayer;
    [SerializeField] private LayerMask _buildingLayer;
    [SerializeField] private float _checkRadius = 0.3f;
    [SerializeField] private Transform _playerMinY;
    [SerializeField] private float _jumpForce = 3f;
    [SerializeField] private float _initialAnimationSpeed;


    public static bool IsShooting = false;
    

    private bool _isGrounded;
    private float _axisX;
    private float _axisY;

    private Animator _playerAnimator;
    private Rigidbody2D _rigid;
    
    public enum eDirection { right, left}
    public eDirection Direction;


    void Awake()
    {
        _rigid = GetComponent<Rigidbody2D>();
        _playerAnimator = GetComponent<Animator>();
    }
    void Start()
    {       
        _initialAnimationSpeed = _playerAnimator.speed;     
    }

    
    void Update()
    {
        Move();
        Jump();
        AnimationController();               
    }

    void FixedUpdate()
    {
        _isGrounded = Physics2D.OverlapCircle(_playerMinY.position, _checkRadius, _groundLayer);
    }

    public void Move()
    {
        _axisX = joystick.Horizontal;
        _axisY = joystick.Vertical;
        if (joystick.Direction.x != 0)
        {
            _rigid.velocity = new Vector2(_axisX * _speed, _rigid.velocity.y);
        }
        else if (joystick.Direction.x == 0)
        {
            _rigid.velocity = new Vector2(0f, _rigid.velocity.y);
        }
        if (joystick.Direction.x > 0 && Player.IsShooting == false)
        {
            transform.localScale = new Vector3 (0.6f,0.6f,1f);
            Direction = eDirection.right;  // Comment it bcz testing fix for shooting bug
        }
        if (joystick.Direction.x < 0 && Player.IsShooting == false)
        {
            transform.localScale = new Vector3(-0.6f, 0.6f, 1f);
            Direction = eDirection.left; // watch upside
        }
    }

    public void Jump()
    {
        if (_axisY > .5f && _isGrounded == true)
        {    
            _rigid.velocity = Vector2.up * _jumpForce;
        }

    }

    public void AnimationController()
    {
        if (_isGrounded && joystick.Direction.x == 0)
        {
            _playerAnimator.SetBool("IsIdling", true);
            _playerAnimator.SetBool("IsWalking", false);
            _playerAnimator.speed = _initialAnimationSpeed;
        }
        if (_isGrounded && joystick.Direction.x != 0)
        {
            _playerAnimator.SetBool("IsIdling", false);
            _playerAnimator.SetBool("IsWalking", true);
            _playerAnimator.speed = Mathf.Sqrt((Mathf.Abs((_speed * _axisX))));
        }
        if (_axisY > .5f && _isGrounded == true)
        {
            _playerAnimator.SetBool("IsJumping", true);
        }
        else { _playerAnimator.SetBool("IsJumping", false); }

        if (IsShooting == true)
        {
            _playerAnimator.SetBool("IsAiming", true);
        }
        else 
        {
            _playerAnimator.SetBool("IsAiming", false);
        }
    }
}
