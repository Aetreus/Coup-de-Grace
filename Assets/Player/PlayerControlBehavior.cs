using System.Collections;
using System.Reflection;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(FlightBehavior), typeof(WeaponManager))]
[RequireComponent(typeof(PlayerTargetSystem))]
public class PlayerControlBehavior : MonoBehaviour {
    FlightBehavior fb;

    WeaponManager wm;

    PlayerTargetSystem pt;

    public string CanvasName = "Canvas";
    public string AoALabelName = "AoALabel/AoAValue";
    public string AltLabelName = "AltLabel/AltValue";
    public string SpdOutputName = "SpdLabel/SpdValue";
    public string SlpOutputName = "SlpLabel/SlpValue";
    public string WpnHolderName = "WpnHolder";
    public string AlertLabelName = "AlertLabel";

    public Rect worldBounds = new Rect(-10000, -10000, 20000, 20000);
    public float killTime = 20;
    public float minWarningTime = 5;
    public float retryTime = 10;

    private GameObject AoAOutput;
    private GameObject AltOutput;
    private GameObject SpdOutput;
    private GameObject SlpOutput;
    private GameObject WpnHolder;
    private GameObject AlertOutput;
    private GameObject canvas;

    public Color warningColor = Color.red;
    public Color defaultColor = Color.green;

    private Text AoALabel;
    private Text AltLabel;
    private Text SpdLabel;
    private Text SlpLabel;

    private List<GameObject> WpnGraphics;

    private Queue<AudioSource> queuedAlerts;
    private AudioSource playing;
    private Vector3 startLocation;
    private Quaternion startRotation;
    private Vector3 startVelocity;
    private float killTimer;
    private float warnTimer;
    private GameObject escMenu;

    private float postKillTimer;
    private bool isDead;

    //Defines what a warning consists of
    [System.Serializable]
    public class Warning : ISerializationCallbackReceiver
    {
        //Object that will be inspected to get the monitored value
        public GameObject reference;
        //Name of component that the value referenced is on
        public string component;
        //Name of the method or variable that is monitored
        public string valueName;
        //Is the name of a method(false means it's a variable)
        public bool isMethod;
        //Parameters passed to the method
        public object[] parameters;



        //Limit check.
        public bool isGreater;
        public float limit;

        //Warning text that is displayed when the warning triggers(note that only the highest priority warning is shown)
        public string warningText;
        //Name of UI element that should be colored to indicate a warning state
        public string warningTargetName;

        internal MethodInfo info;
        internal FieldInfo field;
        internal PropertyInfo prop;

        internal GameObject warningTarget;
        internal bool active = false;
        internal float warnTimer;

        public AudioSource sound;
        public bool forcePlay;

        public Warning(GameObject refer, string comp, string value, bool method, object[] param, bool greater, float limit, string warning, string targetName, AudioSource sound, bool forcePlay)
        {
            reference = refer;
            component = comp;
            valueName = value;
            isMethod = method;
            if (param != null)
                parameters = (object[])param.Clone();
            else
                parameters = null;
            this.limit = limit;
            isGreater = greater;
            warningText = warning;
            warningTargetName = targetName;
            this.sound = sound;
            this.forcePlay = forcePlay;
        }

        public Warning(GameObject refer, string comp, string value, bool method, object[] param, bool greater, float limit, string warning, string targetName) : this(refer, comp, value, false, null, greater, limit, warning, targetName, null, false)
        {

        }

        public Warning(GameObject refer, string comp, string value, bool greater, float limit, string warning, string targetName) : this(refer, comp, value, false, null, greater, limit, warning, targetName)
        {

        }

        public Warning() : this(null, "", "", false, null, true, 0, "", "")
        {

        }

        public Warning(Warning w)
        {
            reference = w.reference;
            component = w.component;
            valueName = w.valueName;
            isMethod = w.isMethod;
            if (w.parameters != null)
                parameters = (object[])w.parameters.Clone();
            else
                parameters = null;
            warningText = w.warningText;
            isGreater = w.isGreater;
            limit = w.limit;
            warningText = w.warningText;
            warningTargetName = w.warningTargetName;
            sound = w.sound;
        }

        public void UpdateWarning()
        {
            System.Type type = reference.GetComponent(component).GetType();
            if (isMethod)
                info = reference.GetType().GetMethod(valueName);
            else if ((prop = type.GetProperty(valueName)) == null)
                field = type.GetField(valueName);
        }

        public void OnBeforeSerialize() { }

        public void OnAfterDeserialize()
        {
        }
    }

    [System.Serializable]
    public class WeaponSpec
    {
        public string nameText;
        public float lockAngle;
        public bool isSupported;
        public GameObject firer;
    }

    public List<Warning> warnings;

    public List<WeaponSpec> weapons;

    private int weaponSelect = 0;

    // Use this for initialization
    void Start() {
        fb = GetComponent<FlightBehavior>();
        wm = weapons[weaponSelect].firer.GetComponent<WeaponManager>();
        pt = GetComponent<PlayerTargetSystem>();


        escMenu = GameObject.Find("EscMenu");
        escMenu.SetActive(false);

        canvas = GameObject.Find(CanvasName);
        AoAOutput = canvas.transform.Find(AoALabelName).gameObject;
        AltOutput = canvas.transform.Find(AltLabelName).gameObject;
        SpdOutput = canvas.transform.Find(SpdOutputName).gameObject;
        SlpOutput = canvas.transform.Find(SlpOutputName).gameObject;
        WpnHolder = canvas.transform.Find(WpnHolderName).gameObject;
        AlertOutput = canvas.transform.Find(AlertLabelName).gameObject;

        AoALabel = AoAOutput.GetComponent<Text>();
        AltLabel = AltOutput.GetComponent<Text>();
        SpdLabel = SpdOutput.GetComponent<Text>();
        SlpLabel = SlpOutput.GetComponent<Text>();

        GetComponent<Rigidbody>().velocity = transform.forward * 150;

        AlertOutput.SetActive(false);

        WpnGraphics = new List<GameObject>();
        queuedAlerts = new Queue<AudioSource>();

        for (int i = 0; i < WpnHolder.transform.childCount; i++)
        {
            WpnGraphics.Add(WpnHolder.transform.GetChild(i).gameObject);
        }

        for (int i = wm.maximumShots; i < WpnHolder.transform.childCount; i++)
        {
            WpnGraphics[i].GetComponent<Image>().color = Color.red;
        }

        startLocation = transform.position;
        startRotation = transform.localRotation;
        startVelocity = GetComponent<Rigidbody>().velocity;

        UpdateWarnings();

        killTimer = killTime;

        isDead = false;
    }

    // Update is called once per frame
    void Update() {
        fb.elevator = Input.GetAxis("Elevator");

        fb.aileron = Input.GetAxis("Aileron");

        fb.throttle = Input.GetAxis("Throttle") * 0.5F + 0.5F;

        AoALabel.text = fb.aoa.ToString();

        AltLabel.text = transform.position.y.ToString();

        SpdLabel.text = fb.airspeed.ToString();

        SlpLabel.text = fb.slip.ToString();

        for (int i = 0; i < WpnGraphics.Count && i < wm.maximumShots; i++)
        {
            WpnGraphics[i].GetComponent<Image>().fillAmount = wm.GetLoadingFraction(i);
        }

        if (Input.GetButtonDown("Fire1"))
        {
            wm.Fire(pt);
        }

        if (Input.GetButtonDown("TargetNext"))
        {
            pt.CycleTarget();
        }

        if (Input.GetButtonDown("TargetClosest"))
        {
            pt.CenterTarget();
        }

        if (Input.GetButtonUp("Escape") && !isDead)
        {
            escMenu.SetActive(!escMenu.activeInHierarchy);
            if (Time.timeScale == 1)
                Time.timeScale = 0;
            else
                Time.timeScale = 1;
                
        }

        if (Input.GetButtonUp("CycleWeapon"))
        {
            weaponSelect++;
            if(weaponSelect > weapons.Count)
            {
                weaponSelect = 0;
            }
            wm = weapons[weaponSelect].firer.GetComponent<WeaponManager>();
            pt.lockAngle = weapons[weaponSelect].lockAngle;
        }

        if(isDead)
        {
            postKillTimer -= Time.unscaledDeltaTime;
            escMenu.transform.Find("EscMenuTitle").GetComponent<Text>().text = "RESPAWN: " + postKillTimer.ToString("F0");
            if (postKillTimer < 0)
                escMenu.transform.Find("Mainmenu").GetComponent<Button>().onClick.Invoke();
        }

        CheckWarnings();

        if (CheckWorldBounds())
        {
            killTime -= Time.deltaTime;
            //TODO: Add the audio alert for being out of bounds here.
            AlertOutput.SetActive(true);
            AlertOutput.GetComponent<Text>().text = "BOUNDS " + killTime.ToString("F1");
        }
        else
            killTime = killTimer;

        if (killTime < 0.0 && !isDead)
            KillScreen();

        warnTimer -= Time.deltaTime;

        if ((playing == null || playing.isPlaying == false) && queuedAlerts.Count > 0)
        {
            playing = queuedAlerts.Dequeue();
            playing.Play();
        }
    }

    //Updates the warning MethodInfo- must be called before a new warning is valid
    public void UpdateWarnings()
    {
        foreach (Warning w in warnings)
        {
            w.UpdateWarning();
            Transform t = canvas.transform.Find(w.warningTargetName);
            if (t != null)
                w.warningTarget = t.gameObject;
            w.warnTimer = minWarningTime;
        }
    }

    void CheckWarnings()
    {
        bool enableWarning = false;
        string warningText = "";
        foreach (Warning w in warnings)
        {
            if(w.reference == null)
            {
                Debug.Log("Player warning object reference evaluated as null and was removed.");
                continue;
            }
            w.warnTimer -= Time.deltaTime;
            float inspect = 0;
            if (w.isMethod)
            {
                inspect = System.Convert.ToSingle(w.info.Invoke(w.reference.GetComponent(w.component), w.parameters));
            }
            else
            {
                if (w.prop != null)
                {
                    var temp = w.prop.GetValue(w.reference.GetComponent(w.component), null);
                    inspect = System.Convert.ToSingle(temp);
                }
                else if (w.field != null)
                {
                    var temp = w.field.GetValue(w.reference.GetComponent(w.component));
                    inspect = System.Convert.ToSingle(temp);
                }
            }
            if (w.isGreater && inspect > w.limit || !w.isGreater && inspect < w.limit)
            {
                enableWarning = true;
                warningText = w.warningText;
                if (w.warningTarget != null)
                {
                    Text[] subLabels = w.warningTarget.GetComponentsInChildren<Text>();
                    foreach (Text t in subLabels)
                    {
                        t.color = warningColor;
                    }
                }
                w.active = true;

                if (w.forcePlay && w.sound != null && !w.sound.isPlaying)
                {
                    w.sound.Play();
                }
                else if (w.sound != null && w.warnTimer <= 0 && !w.forcePlay)
                {
                    w.warnTimer = minWarningTime;
                    if (!queuedAlerts.Contains(w.sound))
                    {
                        queuedAlerts.Enqueue(w.sound);
                    }
                }
            }
            else if (w.active == true)
            {
                if (w.warningTarget != null)
                {
                    Text[] subLabels = w.warningTarget.GetComponentsInChildren<Text>();
                    foreach (Text t in subLabels)
                    {
                        t.color = defaultColor;
                    }
                }
                w.active = false;
                AlertOutput.SetActive(false);

                if (w.sound != null)
                    w.sound.Stop();
            }
        }
        warnings.RemoveAll(w => w.reference == null);
        if (enableWarning)
        {
            AlertOutput.SetActive(true);
            AlertOutput.GetComponent<Text>().text = warningText;
        }
        else
        {
            AlertOutput.SetActive(false);
        }
    }

    private bool CheckWorldBounds()
    {
        if (!worldBounds.Contains(new Vector2(transform.position.x,transform.position.z)))
            return true;
        return false;
    }


    public void Respawn()
    {
        transform.position = startLocation;
        transform.localRotation = startRotation;
        GetComponent<Rigidbody>().velocity = startVelocity;
        GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
    }

    public void KillScreen()
    {
        Time.timeScale = 0.25F;
        escMenu.SetActive(true);
        escMenu.transform.Find("Exit").gameObject.SetActive(false);
        isDead = true;
        postKillTimer = retryTime;
        canvas.transform.Find("ScreenFade").gameObject.GetComponent<Image>().CrossFadeColor(Color.black, 10, true, true);
    }

    void OnDestroy()
    {
        //SceneManager.LoadScene(SceneManager.GetActiveScene().name);

    }
}
