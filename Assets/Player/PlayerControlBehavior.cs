using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(FlightBehavior),typeof(WeaponManager))]
[RequireComponent(typeof(PlayerTargetingScript))]
public class PlayerControlBehavior : MonoBehaviour {
    FlightBehavior fb;

    WeaponManager wm;

    PlayerTargetingScript pt;

    public GameObject AoAOutput;

    public GameObject AltOutput;

    public GameObject SpdOutput;

    public GameObject SlpOutput;

    public GameObject WpnHolder;

    private Text AoALabel;

    private Text AltLabel;

    private Text SpdLabel;

    private Text SlpLabel;

    private List<GameObject> WpnGraphics;

    private List<float> loadTime;

	// Use this for initialization
	void Start () {
        fb = GetComponent<FlightBehavior>();

        wm = GetComponent<WeaponManager>();

        pt = GetComponent<PlayerTargetingScript>();

        WpnGraphics = new List<GameObject>();

        for(int i = 0; i < WpnHolder.transform.childCount; i++)
        {
            WpnGraphics.Add(WpnHolder.transform.GetChild(i).gameObject);
        }

        for(int i = wm.maximumShots; i < WpnHolder.transform.childCount; i++)
        {
            WpnGraphics[i].GetComponent<Image>().color = Color.red;
        }

        AoALabel = AoAOutput.GetComponent<Text>();

        AltLabel = AltOutput.GetComponent<Text>();

        SpdLabel = SpdOutput.GetComponent<Text>();

        SlpLabel = SlpOutput.GetComponent<Text>();

        GetComponent<Rigidbody>().velocity = transform.forward * 150;

    }
	
	// Update is called once per frame
	void Update () {
		fb.elevator = Input.GetAxis("Elevator");

		fb.aileron = Input.GetAxis("Aileron");

        fb.throttle = Input.GetAxis("Throttle");

        AoALabel.text = fb.aoa.ToString();

        AltLabel.text = transform.position.y.ToString();

        SpdLabel.text = GetComponent<Rigidbody>().velocity.magnitude.ToString();

        SlpLabel.text = fb.slip.ToString();

        for(int i = 0; i < WpnGraphics.Count && i < wm.maximumShots; i++)
        {
            WpnGraphics[i].GetComponent<Image>().fillAmount = wm.GetLoadingFraction(i);
        }

        if(Input.GetButtonDown("Fire1"))
        {
            wm.Fire();
        }

        if(Input.GetButtonDown("TargetNext"))
        {
            pt.TargetNext();
        }

        if(Input.GetButtonDown("TargetClosest"))
        {
            pt.TargetCenter();
        }
    }

    void OnDestroy()
    {
        //SceneManager.LoadScene(SceneManager.GetActiveScene().name);

    }
}
