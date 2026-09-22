using UnityEngine;

// Keeps a backdrop glued to the camera with a small drift, so the sky
// still fills the view wherever the player walks, and mountains lag a little.
public class ParallaxCover : MonoBehaviour
{
    [SerializeField] private bool followY = true;
    [SerializeField] private float drift = 0.15f;
    [SerializeField] private float maxDrift = 2.4f;
    [SerializeField] private float lockedWorldY;

    private float _z;

    private void Awake()
    {
        _z = transform.position.z;
    }

    private void LateUpdate()
    {
        var cam = Camera.main;
        if (cam == null) return;

        Vector3 camPos = cam.transform.position;
        float ox = Mathf.Clamp(-camPos.x * drift, -maxDrift, maxDrift);
        float oy = followY ? Mathf.Clamp(-camPos.y * drift * 0.35f, -maxDrift, maxDrift) : 0f;
        float y = followY ? camPos.y + oy : lockedWorldY;
        transform.position = new Vector3(camPos.x + ox, y, _z);
    }
}
