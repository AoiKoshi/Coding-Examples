using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    //Variables
    public GameObject objectToSpawn;
    public float spawnRate;
    float sinceLastSpawn;

    void Start()
    {
        Spawn();
    }

    void Update()
    {
        sinceLastSpawn += Time.deltaTime;
        if (sinceLastSpawn >= spawnRate)
        {
            Spawn();
            sinceLastSpawn = 0f;
        }
    }

    void Spawn()
    {
        GameObject.Instantiate(objectToSpawn, transform.position, Quaternion.identity);
    }
}
