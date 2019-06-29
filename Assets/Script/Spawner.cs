using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour {

    public bool startSpawn = true;

    public float maxSpawnTime = 5;
    public float spawnTime;

    public GameObject enemyTypeObj;
    
	void Start () { }
	
	void Update () {
        if (startSpawn) {
            if (maxSpawnTime > 0.1f) {
                maxSpawnTime -= Time.deltaTime * 0.05f;
            }

            spawnTime += Time.deltaTime;

            if (spawnTime >= maxSpawnTime) {
                Instantiate(enemyTypeObj, transform.position, Quaternion.identity);
                spawnTime = 0;
            }
        }
    }


}
