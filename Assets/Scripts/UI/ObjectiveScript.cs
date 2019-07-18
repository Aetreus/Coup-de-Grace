using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ObjectiveScript : MonoBehaviour {

    protected List<string> objectives;

    protected List<string> clearing;

    protected float clearTimer = 0.0f;

    public float clearTime = 2.5f;

    public Color failColor;

    protected GameObject baseText;

    private void OnEnable()
    {
        //Get the base text and clear it, set it inactive
        baseText = transform.GetChild(0).gameObject;
        baseText.GetComponent<Text>().text = "";
        baseText.SetActive(false);
    }

    //Adds an objective with the given text.
    public void AddObjective(string text)
    {
        if (!objectives.Contains(text))
        {
            GameObject newObjective = Instantiate(baseText, transform);
            newObjective.transform.localPosition = new Vector3(baseText.transform.localPosition.x, baseText.transform.localPosition.y + objectives.Count *-30);
            newObjective.GetComponent<Text>().text = text;
            newObjective.name = text;
            newObjective.SetActive(true);
            objectives.Add(text);
        }
    }

    public void ClearObjective(string text)
    {
        if(objectives.Contains(text))
        {
            clearing.Add(text);
            clearTimer = 0.0f;
        }
    }

    public void FailObjective(string text)
    {
        if(objectives.Contains(text))
        {
            transform.Find(text).GetComponent<Text>().color = failColor;
        }
    }

    public void FailAndClearObjective(string text)
    {
        FailObjective(text);
        ClearObjective(text);
    }

    //Immediately removes all objectives to be cleared
    public void ForceClear()
    {
        foreach (string objective in clearing)
        {
            objectives.Remove(objective);
            Destroy(transform.Find(objective));
        }

        float yLoc = baseText.transform.localPosition.y;
        foreach (string objective in objectives)
        {
            transform.Find(objective).localPosition = new Vector3(transform.Find(objective).localPosition.x, yLoc);
            yLoc -= 30;
        }

        clearing.Clear();
    }

    // Use this for initialization
    void Start () {
        objectives = new List<string>();
        clearing = new List<string>();
	}
	
	// Update is called once per frame
	void Update () {
        if (clearTimer < clearTime)
        {
            clearTimer += Time.deltaTime;
            if (clearTimer >= clearTime)
                ForceClear();
        }
    }
}
