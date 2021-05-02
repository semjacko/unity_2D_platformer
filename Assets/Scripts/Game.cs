using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour
{
    public Transform healthBox;
    public float healthBoxSpawnDelay = 20f;
    private float lastHealthBoxSpawned = 0f;

    // Update is called once per frame
    void Update()
    {
        if (Time.time - this.lastHealthBoxSpawned >= this.healthBoxSpawnDelay)
        {
            float x = Random.Range(1.0f, 17.0f);
            Instantiate(this.healthBox, new Vector3(x, 8f, 30f), Quaternion.identity);
            this.lastHealthBoxSpawned = Time.time;
        }
    }
}
