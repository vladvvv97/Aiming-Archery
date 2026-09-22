using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloudsSpawner : MonoBehaviour
{
    public GameObject[] CloudPrefabs;
    [SerializeField] private bool spawnClouds = true;
    [SerializeField] private float delay = 2f;
    void Start()
    {
        StartCoroutine(Spawner());
    }
    IEnumerator Spawner()
    {
        while (true)
        {
            while(spawnClouds)
            {
                Instantiate(CloudPrefabs[Random.Range(0,CloudPrefabs.Length)]);
                yield return new WaitForSeconds(delay);
            }
        }
    }
}
