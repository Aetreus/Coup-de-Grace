using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjIconScript : MonoBehaviour {

    private List<ProjectileObject> actives;
    private List<RectTransform> icons;
    private RectTransform rt;
    private GameObject canvas;

    public GameObject sourceObject;
    public float referenceDistance;
    public float logBase;

    private void Awake()
    {
        actives = new List<ProjectileObject>();
        icons = new List<RectTransform>();
        rt = GetComponent<RectTransform>();
    }

    // Use this for initialization
    void Start()
    {
        canvas = GameObject.Find("Canvas");
    }

    public void Register(ProjectileObject reg)
    {
        actives.Add(reg);
        GameObject temp = Instantiate(reg.Icon, transform);
        icons.Add(temp.GetComponent<RectTransform>());
        temp.name = reg.Icon.name;
    }

    public void Unregister(ProjectileObject unreg)
    {
        if (actives.Remove(unreg))
        {
            int index = icons.FindIndex(x => x.name == unreg.Icon.name);
            GameObject.Destroy(icons[index].gameObject, 0.1F);
            icons.RemoveAt(index);
        }
    }

    // Update is called once per frame
    void Update()
    {
        foreach (RectTransform r in icons)
        {
            r.gameObject.SetActive(false);
        }

        foreach (ProjectileObject obj in actives)
        {
            RectTransform r = icons.Find(x => x.gameObject.name == obj.Icon.name && !x.gameObject.activeInHierarchy);

            
            Vector3 screenPos = Camera.main.WorldToScreenPoint(obj.transform.position);
            if (screenPos.z > 1)
            {
                r.anchoredPosition = (new Vector2(screenPos.x, screenPos.y) / canvas.GetComponent<RectTransform>().localScale.x) - canvas.GetComponent<RectTransform>().sizeDelta / 2f;


                float distance = (sourceObject.transform.position - obj.transform.position).magnitude;

                if (distance > referenceDistance)
                {
                    float scaleFactor = 1 / (1 + Mathf.Log(distance / referenceDistance) / Mathf.Log(logBase));
                    r.localScale = new Vector3(scaleFactor, scaleFactor, 1);
                }
                else
                {
                    r.localScale = new Vector3(1, 1, 1);
                }
                r.gameObject.SetActive(true);
            }
        }
    }
}
