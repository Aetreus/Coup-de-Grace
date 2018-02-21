using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(FlightBehavior))]
public class PlayerControlBehavior : MonoBehaviour {
    FlightBehavior fb;

    public GameObject AoAOutput;

    public GameObject AltOutput;

    public GameObject SpdOutput;

    private Text AoALabel;

    private Text AltLabel;

    private Text SpdLabel;

	// Use this for initialization
	void Start () {
        fb = GetComponent<FlightBehavior>();

        AoALabel = AoAOutput.GetComponent<Text>();

        AltLabel = AltOutput.GetComponent<Text>();

        SpdLabel = SpdOutput.GetComponent<Text>();

    }
	
	// Update is called once per frame
	void Update () {
		fb.elevator = Input.GetAxis("Elevator");

		fb.aileron = Input.GetAxis("Aileron");

        AoALabel.text = fb.aoa.ToString();

        AltLabel.text = transform.position.y.ToString();

        SpdLabel.text = GetComponent<Rigidbody>().velocity.magnitude.ToString();
    }
}
