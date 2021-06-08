using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZoomPan : MonoBehaviour
{
    Vector3 touch;
    public float zoomMin = 6;
    public float zoomMax = 15;
    void Update()
    {
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
        //else if (Input.GetMouseButton(0))
        //{
        //    Vector3 direction = touch - Camera.main.ScreenToWorldPoint(Input.mousePosition);
        //    Camera.main.transform.position += direction;
        //}
        //Zoom(Input.GetAxis("Mouse ScrollWheel"));
    }

    void Zoom(float increment)
    {
        Camera.main.orthographicSize = Mathf.Clamp(Camera.main.orthographicSize - increment, zoomMin, zoomMax);
    }
}
