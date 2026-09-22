using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCamera : MonoBehaviour
{
    static public GameObject POI; // Ссылка на интересующий объект (Point of interest)
    
    [Header("Set in Inspector")]
    public float easing = 0.05f;
    public Vector2 minXY = Vector2.zero;
    public GameObject Player;

    [Header("Set Dynamically")]
    public float camZ; // Желаемая координата Z камеры

    private void Awake()
    {
        camZ = this.transform.position.z;
    }

    void FixedUpdate()
    {
        //// Однострочная версия if не требует фигурных скобок
        //if (POI == null) return; // выйти, если нет интересуещего объекта

        //// Получить позицию интересуещего объекта
        //Vector3 destination = POI.transform.position;


        Vector3 destination;
        // Если нет интересующего объекта, вернуть Р:[ 0, 0, 0 ]
        if (POI == null)
        {
            destination = Player.transform.position;
        }
        else
        {
            // Получить позицию интересующего объекта
            destination = POI.transform.position;
            // Если интересующий объект - снаряд, убедиться, что он остановился
            if (POI.tag == "Arrow")
            {
                // Если он стоит на месте(то есть не двигается)
                if (POI.GetComponent<Rigidbody2D>().IsSleeping())
                {
                    // Вернуть исходные настройки поля зрения камеры
                    POI = null;
                    //в следующем кадре
                    return;
                }
            }
        }


        // Ограничить Х и У минимальными значениями
        destination.x = Mathf.Max(minXY.x, destination.x);
        destination.y = Mathf.Max(minXY.y, destination.y);

        // Определить точку между текущим местоположением камеры и destination
        destination = Vector3.Lerp(transform.position, destination, easing);

        // Принудительно установить значение destination.z равным camZ, чтобы
        // отодвинуть камеру подальше
        destination.z = camZ;

        // Поместить камеру в позицию destination
        transform.position = destination;

        // Изменить  размер orthographicSize камеры, чтобы земля оставалась в поле зрения
        //Camera.main.orthographicSize = destination.y + 6;
    }
}
