using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainScript : MonoBehaviour {
	private GameObject player;
	private Transform playert;
	private Transform terraint;
	private Vector3 terraincenter;

	public float distancethreshold;

	// Use this for initialization
	void Start () {
		player = GameObject.Find("First Person Controller");
		playert = player.GetComponent<Transform>();
		terraint = gameObject.GetComponent<Transform>();
		updateterraincenter();
	}
	
	// Update is called once per frame
	void Update () {
		float distance = (playert.position - terraincenter).magnitude;
		float xdistance = playert.position.x - terraincenter.x;
		float zdistance = playert.position.z - terraincenter.z;

		print("xdistance: " + xdistance);
		print("zdistance: " + zdistance);
		print("total distance: " + distance);

		if (xdistance >= distancethreshold) {
			terraint.position += new Vector3(distancethreshold,0,0);
			updateterraincenter();
		}
		if (xdistance <= -distancethreshold) {
			terraint.position -= new Vector3(distancethreshold,0,0);
			updateterraincenter();
		}
		if (zdistance >= distancethreshold) {
			terraint.position += new Vector3(0,0,distancethreshold);
			updateterraincenter();
		}
		if (zdistance <= -distancethreshold) {
			terraint.position -= new Vector3(0,0,distancethreshold);
			updateterraincenter();
		}
	}

	void updateterraincenter() {
		Vector3 terrainsize = gameObject.GetComponent<Terrain>().terrainData.size;
		terraincenter = terraint.position + new Vector3(terrainsize.x/2, 0, terrainsize.z/2);
	}
}
