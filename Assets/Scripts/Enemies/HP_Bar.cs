using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HP_Bar : MonoBehaviour
{
    private Enemy _enemy;
    private SpriteRenderer _spriteRenderer;
    private float _percentHP;

    void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _enemy = GetComponentInParent<Enemy>();
    }
    void Update()
    {
        _percentHP = _enemy.Health / _enemy.MaxHealth;
        transform.localScale = new Vector3(_percentHP, 1,1);
        if (_percentHP > 0.66)
        { _spriteRenderer.color = Color.green; }
        else if (_percentHP <= 0.66 && _percentHP >= 0.33)
        { _spriteRenderer.color = Color.yellow; }
        else if (_percentHP < 0.33)
        { _spriteRenderer.color = Color.red; }

    }

}
