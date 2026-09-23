using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZoomPan : MonoBehaviour
{
    Vector3 touch;
    public float zoomMin = 6;
    public float zoomMax = 15;
    [SerializeField] private float scrollZoomSpeed = 1.25f;

    void Update()
    {
        if (Camera.main == null) return;

        //if (Input.GetMouseButtonDown(0))
        //{
        //    touch = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        //}

        if (Input.touchCount == 2 && Player.IsShooting == false)
        {
            Touch touchZero = Input.GetTouch(0);
            Touch touchOne = Input.GetTouch(1);

            Vector2 touchZeroLastPos = touchZero.position - touchZero.deltaPosition;
            Vector2 touchOneLastPos = touchOne.position - touchOne.deltaPosition;

            float distanceTouch = (touchZeroLastPos - touchOneLastPos).magnitude;
            float currentDistTouch = (touchZero.position - touchOne.position).magnitude;

            float difference = currentDistTouch - distanceTouch;

            Zoom(difference * 0.01f);
        }

        // Wheel notch and a laptop trackpad pinch both arrive as scroll delta.
        float scroll = Input.mouseScrollDelta.y;
        if (Mathf.Abs(scroll) > 0.01f && Player.IsShooting == false)
            Zoom(scroll * scrollZoomSpeed);

        //else if (Input.GetMouseButton(0))
        //{
        //    Vector3 direction = touch - Camera.main.ScreenToWorldPoint(Input.mousePosition);
        //    Camera.main.transform.position += direction;
        //}
    }

    void Zoom(float increment)
    {
        Camera.main.orthographicSize = Mathf.Clamp(Camera.main.orthographicSize - increment, zoomMin, zoomMax);
    }
}
