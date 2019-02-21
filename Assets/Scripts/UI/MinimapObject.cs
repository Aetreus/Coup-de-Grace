using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MinimapObject : MonoBehaviour {


    private MinimapScript ms;
    private bool isAdded = false;
    private PropNav pn;

    private RectTransform instantiatedIcon;
    private Image iconSprite;

    [SerializeField]
    private GameObject _icon;

    public GameObject Icon
    {
        get { return _icon; }
        private set { _icon = value; }
    }

    void Awake()
    {
        //tryAdd();
    }
        
	// Use this for initialization
	void Start () {
        tryAdd();
        pn = GetComponent<PropNav>();
	}

    public void tryAdd()
    {
        if (!isAdded)
        {
            ms = FindObjectOfType<MinimapScript>();
            if (ms.isAwake)
            {
                _icon = ms.Register(this);
                isAdded = true;
                iconSprite = _icon.GetComponent<Image>();
            }
        }
    }
	
	// Update is called once per frame
	void Update () {
        //If this is a missile tracking the player it is red, otherwise missiles are white.
		if(pn != null)
        {
            if (pn.target != null && pn.target.tag == "Player")
            {
                iconSprite.color = Color.red;
            }
            else
                iconSprite.color = Color.white;
        }
	}

    private void OnDestroy()
    {
        if(ms != null)
            ms.Unregister(this);
    }
}
