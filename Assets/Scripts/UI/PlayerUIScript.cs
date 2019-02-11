using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(PlayerControlBehavior))]
public class PlayerUIScript : MonoBehaviour {

    public string CanvasName = "Canvas";
    public string AoALabelName = "AoALabel/AoAValue";
    public string AltLabelName = "AltLabel/AltValue";
    public string SpdOutputName = "SpdLabel/SpdValue";
    public string SlpOutputName = "SlpLabel/SlpValue";
    public string WpnHolderName = "WpnHolder";
    public string AlertLabelName = "AlertLabel";
    public string HealthHolderName = "HealthUIHolder";

    private GameObject AoAOutput;
    private GameObject AltOutput;
    private GameObject SpdOutput;
    private GameObject SlpOutput;
    private GameObject WpnHolder;
    private GameObject AlertOutput;
    private GameObject canvas;
    private GameObject HealthOutput;

    private Text AoALabel;
    private Text AltLabel;
    private Text SpdLabel;
    private Text SlpLabel;

    private FlightBehavior fb;

    // Use this for initialization
    void Start () {

        fb = GetComponent<FlightBehavior>();

        canvas = GameObject.Find(CanvasName);
        AoAOutput = canvas.transform.Find(AoALabelName).gameObject;
        AltOutput = canvas.transform.Find(AltLabelName).gameObject;
        SpdOutput = canvas.transform.Find(SpdOutputName).gameObject;
        SlpOutput = canvas.transform.Find(SlpOutputName).gameObject;
        WpnHolder = canvas.transform.Find(WpnHolderName).gameObject;
        AlertOutput = canvas.transform.Find(AlertLabelName).gameObject;
        HealthOutput = canvas.transform.Find(HealthHolderName).gameObject;

        AoALabel = AoAOutput.GetComponent<Text>();
        AltLabel = AltOutput.GetComponent<Text>();
        SpdLabel = SpdOutput.GetComponent<Text>();
        SlpLabel = SlpOutput.GetComponent<Text>();

    }
	
	// Update is called once per frame
	void Update () {

        AoALabel.text = fb.aoa.ToString();

        AltLabel.text = transform.position.y.ToString();

        //Convert m/s to km/h
        SpdLabel.text = (fb.airspeed * 3.6F).ToString();

        SlpLabel.text = fb.slip.ToString();
    }
}
