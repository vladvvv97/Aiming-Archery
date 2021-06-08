using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class Enemy : MonoBehaviour
{
    public Transform Head;
    public Transform Chest;
    public Transform Legs;
    public Transform Shield = null;
    public enum eBoydPart { head, chest, legs, shield };
    
    [SerializeField] private float health = 100;
    [SerializeField] private float _maxHealth;
    [SerializeField] private float headMultiplier = 2f;
    [SerializeField] private float chestMultiplier = 1f;
    [SerializeField] private float legsMultiplier = 0.5f;
    [SerializeField] private float hitStartTime = -999f;
    [SerializeField] private float hitEndTime = 0.5f;

    private Animator _animator;
    private SpriteRenderer[] _spriteRenderer;
    private Collider2D[] _colliders;
    private Rigidbody2D _rb2D;




    public float Health { get { return health; }  private set { health = value; if (health < 0) health = 0; } }
    public float MaxHealth { get { return _maxHealth; } set { _maxHealth = value; } }
     protected virtual void Awake()
    {
        _colliders = GetComponentsInChildren<Collider2D>();
        _animator = GetComponent<Animator>();
        _spriteRenderer = GetComponentsInChildren<SpriteRenderer>();
    }

    protected virtual void Start()
    {
        MaxHealth = Health;
    }


    protected virtual void Update()
    {
        foreach (var sr in _spriteRenderer)
        {
            if (sr.sortingLayerName != "HP Bar")
            sr.color = (hitStartTime + hitEndTime > Time.time) ? Color.red : Color.white; 
        }

        
    }

    public void Damaged(eBoydPart damagedPart, float dmg, Transform arrow = null, bool alreadyHit = false)
    {
        if (alreadyHit)
        { return; };

        hitStartTime = Time.time;

        if (damagedPart == eBoydPart.head)
        {
            Health -= dmg * headMultiplier;
            if (arrow)
            {
                arrow.SetParent(Head);
            }
        }
        else if (damagedPart == eBoydPart.chest)
        {
            Health -= dmg * chestMultiplier;
            if (arrow)
            {
                arrow.SetParent(Chest);
            }
        }
        else if (damagedPart == eBoydPart.legs)
        {
            Health -= dmg * legsMultiplier;
            if (arrow)
            {
                arrow.SetParent(Legs);
            }
        }

        else
        {
            return;
        }

        if (Health <= 0)
        {
            _animator.SetTrigger("IsDied");
            Die();
        }

        if (UnityEngine.Random.Range(0,2) == 0)
        {
            AudioManager.INSTANCE.PlaySound(AudioManager.eSoundsNames.enemy_hurt1);
        }
        else
        {
            AudioManager.INSTANCE.PlaySound(AudioManager.eSoundsNames.enemy_hurt2);
        }
        
    }

    public void Block(eBoydPart damagedPart, Transform arrow = null)
    {
        if (damagedPart == eBoydPart.shield)
        {
            if (arrow)
            {
                arrow.SetParent(Shield);
            }
        }
        AudioManager.INSTANCE.PlaySound(AudioManager.eSoundsNames.arrow_hit_wood);
    }

    protected virtual void Die()
    {
        //if (_rb2D)
        //{
        //    _rb2D.isKinematic = true;            
        //}

        //foreach (var coll in _colliders)
        //{
        //    coll.enabled = false;
        //}

        Destroy(gameObject, 2f);
    }
}
