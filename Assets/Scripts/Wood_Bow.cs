using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wood_Bow : MonoBehaviour
{
    [Header("Set In Inspector: Wood Bow")]
    public GameObject arrowPrefab;
    public Sprite[] _bowSprites;
    public Player Player;
    public Joystick joystick;
    public GameObject point;
    public Transform shotPoint;
    public Transform rightArmBone;          //6
    public Transform rightHandBone;         //7
    public Transform _holdHandTransform;    //8
    public Transform leftHandBone;          //9
    public Transform neckBone;              //3

    [SerializeField] private Vector3 _defaultArrowOffset = new Vector3(0.2f, 0f, 0);
    [SerializeField] private Vector3 _defaultArrowOffset0 = new Vector3(1f, 0f, 0);
    [SerializeField] private Vector3 _defaultArrowOffset1 = new Vector3(0.85f, 0f, 0);
    [SerializeField] private Vector3 _defaultArrowOffset2 = new Vector3(0.7f, 0f, 0);
    [SerializeField] private Vector3 _defaultArrowOffset3 = new Vector3(0.55f, 0f, 0);
    [SerializeField] private Vector3 _defaultArrowOffset4 = new Vector3(0.35f, 0f, 0);
    [SerializeField] private Vector3 _defaultArrowOffset5 = new Vector3(0.2f, 0f, 0);

    [SerializeField] private Vector3 _defaultBowOffset = new Vector3(0.3f, 0.1f, 0);
    [SerializeField] private Vector3 _defaultBowOffset0 = new Vector3(-0.3f, 0.1f, 0);
    [SerializeField] private Vector3 _defaultBowOffset1 = new Vector3(-0.25f, 0.1f, 0);
    [SerializeField] private Vector3 _defaultBowOffset2 = new Vector3(-0.2f, 0.1f, 0);
    [SerializeField] private Vector3 _defaultBowOffset3 = new Vector3(-0.1f, 0.1f, 0);
    [SerializeField] private Vector3 _defaultBowOffset4 = new Vector3(0.1f, 0.1f, 0);
    [SerializeField] private Vector3 _defaultBowOffset5 = new Vector3(0.3f, 0.1f, 0);

    [SerializeField] public float _arrowImpulseForce;
    [SerializeField] public float _arrowImpulseForce0;
    [SerializeField] public float _arrowImpulseForce1;
    [SerializeField] public float _arrowImpulseForce2;
    [SerializeField] public float _arrowImpulseForce3;
    [SerializeField] public float _arrowImpulseForce4;
    [SerializeField] public float _arrowImpulseForce5;

    [SerializeField] private int _numOfPoints;
    [SerializeField] private float _spaceBetweenPoints;
    [SerializeField] private float _delayBtwShots = 1.0f;
    [SerializeField] private float _timer;
    [SerializeField] private bool shotFired;

    private SpriteRenderer _spriteRendererBow;
    private Vector2 direction;
    private Vector2 _inputVector;
    private Animator _animator;
    private Animator _playerAnimator;
    private Wood_Arrow _currentArrow;
    private GameObject[] points;
    private Rigidbody2D rigid;


    private Vector3 _initialBowPosition;
    private Quaternion _initialBowRotation;
    [SerializeField] private Vector3 _initialShotPointPos;
    private int _initialOrderInLayer = 2;


    void Awake()
    {
        _animator = GetComponent<Animator>();
        _spriteRendererBow = GetComponent<SpriteRenderer>();
        _playerAnimator = Player.GetComponent<Animator>();
    }
    void Start()
    {

        _initialBowPosition = transform.localPosition;
        _initialBowRotation = transform.localRotation;
        _initialOrderInLayer = _spriteRendererBow.sortingOrder;
        _initialShotPointPos = shotPoint.localPosition;
        points = new GameObject[_numOfPoints];
        for (int i = 0; i < _numOfPoints; i++)
        {
            points[i] = Instantiate(point, shotPoint.position, Quaternion.identity);
            points[i].SetActive(false);
        }
    }
    void Update()
    {

        if (Player.IsShooting)
        {
            _inputVector = new Vector2(joystick.Horizontal, joystick.Vertical);
            Vector2 bowPosition = Vector2.zero;
            direction = _inputVector - bowPosition;
            transform.right = direction;

            _playerAnimator.enabled = false;

            if (Player.Direction == Player.eDirection.left)
            {
                neckBone.transform.up = direction;
                if (direction.y < -0.1)
                {
                    neckBone.transform.localRotation = Quaternion.Euler(0, 0, Mathf.Clamp(neckBone.transform.localRotation.eulerAngles.z, 0f, 45f));
                }
                else if (direction.y > 0.1)
                {
                    neckBone.transform.localRotation = Quaternion.Euler(0, 0, Mathf.Clamp(neckBone.transform.localRotation.eulerAngles.z, 315f, 360f));

                }
                else
                {
                    neckBone.transform.localRotation = Quaternion.Euler(0, 0, 0);
                }

                rightArmBone.transform.right = -direction;
                rightHandBone.transform.right = direction;
                leftHandBone.transform.right = direction;
                _holdHandTransform.transform.right = direction;
                rightHandBone.transform.rotation = Quaternion.Euler(0, 0, -20);
                rightHandBone.transform.localScale = new Vector3(0.7f, 1, 0);
                leftHandBone.transform.localScale = new Vector3(1.3f, 1, 0);

                _currentArrow.transform.localPosition = _defaultArrowOffset;
                transform.localPosition = -_defaultBowOffset;
            }
            if (Player.Direction == Player.eDirection.right)
            {
                neckBone.transform.up = direction;
                if (direction.y < -0.1)
                {
                    neckBone.transform.localRotation = Quaternion.Euler(0, 0, Mathf.Clamp(neckBone.transform.localRotation.eulerAngles.z, 0f, 45f));
                }
                else if (direction.y > 0.1)
                {
                    neckBone.transform.localRotation = Quaternion.Euler(0, 0, Mathf.Clamp(neckBone.transform.localRotation.eulerAngles.z, 315f, 360f));

                }
                else
                {
                    neckBone.transform.localRotation = Quaternion.Euler(0, 0, 0);
                }

                rightArmBone.transform.right = direction;
                rightHandBone.transform.right = -direction;
                leftHandBone.transform.right = -direction;
                _holdHandTransform.transform.right = -direction;
                rightHandBone.transform.rotation = Quaternion.Euler(0, 0, 20);
                rightHandBone.transform.localScale = new Vector3(0.7f, 1, 0);
                leftHandBone.transform.localScale = new Vector3(1.3f, 1, 0);

                _currentArrow.transform.localPosition = -_defaultArrowOffset;
                transform.localPosition = -_defaultBowOffset;
            }


            if (joystick.Horizontal < 0 && joystick.Vertical != 0)
            {
                Player.transform.localScale = new Vector3(0.6f, 0.6f, 1f);
                Player.Direction = Player.eDirection.right;
                _spriteRendererBow.flipX = true;
                _spriteRendererBow.flipY = true;
                _currentArrow.transform.localRotation = Quaternion.Euler(0, 0, 90);

            }
            else if (joystick.Horizontal < 0 && joystick.Vertical == 0)
            {
                transform.localRotation = Quaternion.Euler(0, 0, -180);
            }
            else if (joystick.Horizontal > 0 && joystick.Vertical != 0)
            {
                Player.transform.localScale = new Vector3(-0.6f, 0.6f, 1f);
                Player.Direction = Player.eDirection.left;
                _spriteRendererBow.flipX = false;
                _spriteRendererBow.flipY = false;
                _currentArrow.transform.localRotation = Quaternion.Euler(0, 0, -90);
            }
            else
            {
                if (Player.Direction == Player.eDirection.right && joystick.Horizontal == 0 && joystick.Vertical == 0)
                {
                    Player.transform.localScale = new Vector3(0.6f, 0.6f, 1f);
                    _spriteRendererBow.flipX = false;
                    _spriteRendererBow.flipY = false;
                    _currentArrow.transform.localRotation = Quaternion.Euler(0, 0, -90);
                    _currentArrow.transform.localPosition = _defaultArrowOffset;
                    transform.localPosition = -_defaultBowOffset;
                }
                if (Player.Direction == Player.eDirection.left && (joystick.Horizontal == 0 && joystick.Vertical == 0))
                {
                    _currentArrow.transform.localRotation = Quaternion.Euler(0, 0, -90);
                }
            }


            for (int i = 0; i < _numOfPoints; i++)
            {
                if (Player.Direction == Player.eDirection.left)
                {
                    shotPoint.localPosition = _initialShotPointPos;
                    points[i].SetActive(true);
                    points[i].transform.position = PointPosition((i + 2) * _spaceBetweenPoints);
                }
                if (Player.Direction == Player.eDirection.right)
                {
                    shotPoint.localPosition = -_initialShotPointPos;
                    points[i].SetActive(true);
                    points[i].transform.position = PointPosition((i + 2) * _spaceBetweenPoints);
                }
            }


            if (_inputVector.magnitude == 0)
            {
                _spriteRendererBow.sprite = _bowSprites[0];
                _defaultArrowOffset = _defaultArrowOffset0;
                _defaultBowOffset = _defaultBowOffset0;
                _arrowImpulseForce = _arrowImpulseForce0;
            }
            else if (_inputVector.magnitude > 0 && _inputVector.magnitude <= 0.2)
            {
                _spriteRendererBow.sprite = _bowSprites[1];
                _defaultArrowOffset = _defaultArrowOffset1;
                _defaultBowOffset = _defaultBowOffset1;
                _arrowImpulseForce = _arrowImpulseForce1;
            }
            else if (_inputVector.magnitude > 0.2 && _inputVector.magnitude <= 0.4)
            {
                _spriteRendererBow.sprite = _bowSprites[2];
                _defaultArrowOffset = _defaultArrowOffset2;
                _defaultBowOffset = _defaultBowOffset2;
                _arrowImpulseForce = _arrowImpulseForce2;
            }
            else if (_inputVector.magnitude > 0.4 && _inputVector.magnitude <= 0.6)
            {
                _spriteRendererBow.sprite = _bowSprites[3];
                _defaultArrowOffset = _defaultArrowOffset3;
                _defaultBowOffset = _defaultBowOffset3;
                _arrowImpulseForce = _arrowImpulseForce3;
            }
            else if (_inputVector.magnitude > 0.6 && _inputVector.magnitude <= 0.8)
            {
                _spriteRendererBow.sprite = _bowSprites[4];
                _defaultArrowOffset = _defaultArrowOffset4;
                _defaultBowOffset = _defaultBowOffset4;
                _arrowImpulseForce = _arrowImpulseForce4;
            }
            else if (_inputVector.magnitude > 0.8 && _inputVector.magnitude <= 1)
            {
                _spriteRendererBow.sprite = _bowSprites[5];
                _defaultArrowOffset = _defaultArrowOffset5;
                _defaultBowOffset = _defaultBowOffset5;
                _arrowImpulseForce = _arrowImpulseForce5;
            }

        }

        if (!Player.IsShooting)
        {
            for (int i = 0; i < _numOfPoints; i++)
            { points[i].SetActive(false); }
        }

        if ((_timer += Time.deltaTime) >= _delayBtwShots)
        {
            if (shotFired)
            {
                _timer = 0.0f;
                joystick.gameObject.SetActive(false);
            }

            else { joystick.gameObject.SetActive(true); }
        }

    }

    private Vector2 PointPosition(float t)
    {
        Vector2 position = (Vector2)shotPoint.position + (direction.normalized * -_arrowImpulseForce * t)
         + 0.5f * Physics2D.gravity * (Mathf.Pow(t, 2));
        return position;
    }
    public void OnPointerDown()
    {
        Player.IsShooting = true;
        OnDrag();
        AudioManager.INSTANCE.PlaySound(AudioManager.eSoundsNames.bow_string_pull);
    }
    public void OnDrag()
    {

        _animator.enabled = false;
        gameObject.transform.SetParent(_holdHandTransform);

        transform.localPosition = _defaultBowOffset;
        transform.localRotation = Quaternion.Euler(0, 0, 180);
        _spriteRendererBow.sprite = _bowSprites[0];
        _spriteRendererBow.sortingOrder = 9;



        _currentArrow = Instantiate<GameObject>(arrowPrefab, transform).GetComponent<Wood_Arrow>(); // transform
    }

    public void ShootOnPointerUp()
    {
        AudioManager.INSTANCE.PlaySound(AudioManager.eSoundsNames.bow_shoot);

        _animator.enabled = true;
        rigid = _currentArrow.GetComponent<Rigidbody2D>();
        _currentArrow.transform.parent = null;
        rigid.simulated = true;
        rigid.AddForce(transform.right * -_arrowImpulseForce, ForceMode2D.Impulse);
        MainCamera.POI = _currentArrow.gameObject;
        gameObject.transform.SetParent(Player.transform);

        _animator.Play("Wood_Bow_Animation_Reverse", -1, 0);
        _spriteRendererBow.sprite = _bowSprites[0];
        Invoke("HideWeapon", _delayBtwShots);


        Player.IsShooting = false;
        shotFired = true;
        Invoke("SetShotFiredFalse", 0.1f);
    }



    private void SetShotFiredFalse()
    {
        shotFired = false;
    }

    public void HideWeapon()
    {

        _spriteRendererBow.sortingOrder = _initialOrderInLayer;
        transform.localPosition = _initialBowPosition;
        transform.localRotation = _initialBowRotation;
        _spriteRendererBow.flipX = false;
        _spriteRendererBow.flipY = false;
        _playerAnimator.enabled = true;
        rightHandBone.transform.localScale = new Vector3(1, 1, 0);
        leftHandBone.transform.localScale = new Vector3(1, 1, 0);
    }
}
