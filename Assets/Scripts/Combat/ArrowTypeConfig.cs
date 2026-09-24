using UnityEngine;

// Per-type projectile data (see SPEC.md 2.2). Ignite and freeze are stored for later phases and unused here.
[CreateAssetMenu(fileName = "ArrowTypeConfig", menuName = "Archery/Arrow Type Config")]
public class ArrowTypeConfig : ScriptableObject
{
    public ArrowTypeId id = ArrowTypeId.Normal;
    public string displayNameKey = "arrow.normal";
    public Sprite icon;
    public GameObject prefab;
    public float baseDamage = 50f;
    public float aoeRadius;
    public int pierceCharges;
    [Tooltip("1 = normal arc. Piercing uses a small value so the shot stays almost straight.")]
    public float gravityScale = 1f;
    public bool ignite;
    public float igniteDuration;
    public float igniteDps;
    public float freezeDuration;
    public Color trailColor = Color.white;
}
