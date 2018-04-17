using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveSpawnerScript : MonoBehaviour {

    public float spawnDelayTime;
    public bool autoSpawn;
    public bool limitWaves;
    public float spawnDelayTimer;
    public int waves;

    

	// Use this for initialization
	void Start () {
        spawnDelayTimer = spawnDelayTime;
        foreach(Transform child in transform)
        {
            GameObject g = child.gameObject;
            g.SetActive(false);
        }
	}
	
	// Update is called once per frame
	void Update () {
		if(autoSpawn && (!limitWaves || waves > 0))
        {
            spawnDelayTimer -= Time.deltaTime;
            if(spawnDelayTimer < 0)
            {
                spawnDelayTimer = spawnDelayTime;
                waves--;
                SpawnWave();
            }
        }
	}

    public void SpawnWave()
    {
        foreach (Transform child in transform)
        {
            GameObject g = child.gameObject;
            g = Instantiate(g,g.transform.position,g.transform.rotation);
            g.SetActive(true);
        }
    }
}
