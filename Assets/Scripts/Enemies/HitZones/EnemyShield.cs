using UnityEngine;

// Shield durability. While durability > 0 a non-piercing hit chips the shield and does not reach the body.
public class EnemyShield : MonoBehaviour
{
    [SerializeField] private int durability = 2;

    private Collider2D _collider;
    private SpriteRenderer _sprite;

    public int Durability => durability;

    private void Awake()
    {
        _collider = GetComponent<Collider2D>();
        _sprite = GetComponent<SpriteRenderer>();
    }

    public void ReduceDurability(int amount = 1)
    {
        if (durability <= 0) return;
        durability = Mathf.Max(0, durability - amount);
        if (durability <= 0)
            DisableShield();
    }

    private void DisableShield()
    {
        if (_collider == null) _collider = GetComponent<Collider2D>();
        if (_sprite == null) _sprite = GetComponent<SpriteRenderer>();
        if (_collider != null) _collider.enabled = false;
        if (_sprite != null) _sprite.enabled = false;
    }
}
