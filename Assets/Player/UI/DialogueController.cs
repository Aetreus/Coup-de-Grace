using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueController : MonoBehaviour {
    public string PlayerTag = "Player";
    public string DialoguePlayerName = "DialoguePlayer";


    public List<LineEntry> linesInit;

    [System.Serializable]
    public class LinesDict : Dictionary<string, DialogueLine> { }
    public LinesDict lines = new LinesDict();


    private AudioSource dialoguePlayer;
    private Image outputImage;
    private Text outputText;
    private float lineTimer;
    private string nextLine;

    public bool Playing { get { return lineTimer > 0; } }

    [System.Serializable]
    public struct LineEntry
    {
        public string name;
        public DialogueLine line;
    }

    [System.Serializable]
    public struct DialogueLine
    {
        public string text;
        public float time;
        public Sprite speaker;
        public AudioClip voice;
        public string nextLine;
    }

    // Use this for initialization
    void Start() {
        GameObject player = GameObject.FindGameObjectWithTag(PlayerTag);
        dialoguePlayer = player.transform.Find(DialoguePlayerName).gameObject.GetComponent<AudioSource>();
        outputImage = transform.Find("SpeakerImage").GetComponent<Image>();
        outputText = transform.Find("DialogueText").GetComponent<Text>();
        gameObject.SetActive(false);
        foreach (LineEntry e in linesInit)
        {
            lines.Add(e.name, e.line);
        }
        //PlayLine("hello");
	}
	
	// Update is called once per frame
	void Update () {

        if (lineTimer > 0)
        {
            lineTimer -= Time.deltaTime;
            if(lineTimer <= 0)
            {
                if(lines.ContainsKey(nextLine))
                {
                    DialogueLine line = lines[nextLine];
                    lineTimer = line.time;
                    dialoguePlayer.clip = line.voice;
                    outputImage.overrideSprite = line.speaker;
                    outputText.text = line.text;
                    nextLine = line.nextLine;
                    dialoguePlayer.Play();
                }
                else
                    gameObject.SetActive(false);
            }
        }
	}

    public void PlayLine(string name)
    {
        if (lines.ContainsKey(name))
        {
            DialogueLine line = lines[name];
            lineTimer = line.time;
            dialoguePlayer.clip = line.voice;
            outputImage.overrideSprite = line.speaker;
            outputText.text = line.text;
            dialoguePlayer.Play();
            gameObject.SetActive(true);
        }
    }

    public void PlayLineSeries(string name)
    {
        if (lines.ContainsKey(name))
        {
            DialogueLine line = lines[name];
            lineTimer = line.time;
            dialoguePlayer.clip = line.voice;
            outputImage.overrideSprite = line.speaker;
            outputText.text = line.text;
            dialoguePlayer.Play();
            nextLine = line.nextLine;
            gameObject.SetActive(true);
        }
    }
}
