using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSaverScript : MonoBehaviour {

    string saved_scene;

	// Use this for initialization
	void Start () {
        DontDestroyOnLoad(this.gameObject);
        saved_scene = null;
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public string SavedScene
    {
        get
        {
            return saved_scene;
        }
        set
        {
            saved_scene = value;
        }
    }

    public void ContinueFromLastLevel()
    {
        if(saved_scene != null)
        {
            SceneManager.LoadScene(saved_scene);
        }
    }
}
