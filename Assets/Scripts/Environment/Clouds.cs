using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Clouds : MonoBehaviour
{
    public Transform StartPoint;
    public Transform EndPoint;
    [SerializeField] private float minSpeed = 0.5f;
    [SerializeField] private float maxSpeed = 3f;
    [SerializeField] private float minY = 7f;
    [SerializeField] private float maxY = 15f;


    private float speed;
    

    void Start()
    {
        speed = Random.Range(minSpeed, maxSpeed);
        transform.position = new Vector3(StartPoint.position.x, Random.Range(minY, maxY), transform.position.z);
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(speed * Time.deltaTime, 0, 0);

        if (transform.position.x >= EndPoint.position.x)
        {
            Destroy(this.gameObject);
        }
    }
}
