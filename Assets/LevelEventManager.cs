using System.Collections;
using System.Reflection;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using System.ComponentModel;

public class LevelEventManager : MonoBehaviour {

    [System.Serializable]
    public class Event
    {
        public FunctionCall condition;
        public bool isConditionBoolean;
        public bool invertCondition;
        public float min;
        public float max;
        public int previousEvent;
        public bool previousFired;
        public bool oneTime;
        private bool fired;
        public bool reset;
        public float delay;
        public float delayTimer;
        public int subsequentEvent;
        public FunctionCall predicate;

        public bool CheckConditions()
        {
            float inspect = 0;
            bool result = false;
            if (isConditionBoolean)
            {
                if (condition.isMethod)
                {
                    ParameterInfo[] args = condition.info.GetParameters();
                    if(args.Length != 0)
                    if(condition.parameters[0].GetType().Equals(args[0].ParameterType.GetElementType()))
                    {
                        int i = 0;
                        List<object> list = new List<object>();
                        while(i < condition.parameters.Length && condition.parameters[i].GetType().Equals(args[0].ParameterType.GetElementType()))
                        {
                            list.Add(condition.parameters[i]);
                            i++;
                        }
                        var arr = Array.CreateInstance(condition.parameters[0].GetType(), list.Count);
                        Array.Copy(list.ToArray(), arr, list.Count);
                        condition.parameters = new object[]{arr};
                    }
                    bool temp = System.Convert.ToBoolean(condition.info.Invoke(condition.reference.GetComponent(condition.component), condition.parameters));
                    result = temp;
                }
                else
                {
                    if (condition.prop != null)
                    {
                        var temp = condition.prop.GetValue(condition.reference.GetComponent(condition.component), null);
                        result = System.Convert.ToBoolean(temp);
                    }
                    else if (condition.field != null)
                    {
                        var temp = condition.field.GetValue(condition.reference.GetComponent(condition.component));
                        result = System.Convert.ToBoolean(temp);
                    }
                }
            }
            else
            {
                if (condition.isMethod)
                {
                    inspect = System.Convert.ToSingle(condition.info.Invoke(condition.reference.GetComponent(condition.component), condition.parameters));
                }
                else
                {
                    if (condition.prop != null)
                    {
                        var temp = condition.prop.GetValue(condition.reference.GetComponent(condition.component), null);
                        inspect = System.Convert.ToSingle(temp);
                    }
                    else if (condition.field != null)
                    {
                        var temp = condition.field.GetValue(condition.reference.GetComponent(condition.component));
                        inspect = System.Convert.ToSingle(temp);
                    }
                }
            }
            if(invertCondition)
            {
                if (result)
                    result = false;
                else
                    result = true;
            }

            return previousFired && (!isConditionBoolean && (inspect >= min && inspect <= max) || result)  && !(oneTime && fired) && !reset;
        }

        public void CheckEvent()
        {
            if(CheckConditions())
            {
                if (predicate.isMethod)
                {
                    if (predicate.component == "GameObject")
                        predicate.info.Invoke(predicate.reference, predicate.parameters);
                    else
                        predicate.info.Invoke(predicate.reference.GetComponent(predicate.component), predicate.parameters);
                }
                else
                {
                    if (condition.prop != null)
                    {
                        if (predicate.component == "GameObject")
                            condition.prop.SetValue(predicate.reference, predicate.parameters[0], null);
                        else
                            condition.prop.SetValue(predicate.reference.GetComponent(predicate.component), predicate.parameters[0],null);
                    }
                    else if (condition.field != null)
                    {
                        if (predicate.component == "GameObject")
                            condition.field.SetValue(predicate.reference, predicate.parameters[0]);
                        else
                            condition.field.SetValue(predicate.reference.GetComponent(predicate.component), predicate.parameters[0]);
                    }
                }
                fired = true;
                reset = true;
                delayTimer = delay;
            }
        }

        public void UpdateEvent()
        {
            condition.UpdateFunctionCall();
            predicate.UpdateFunctionCall();
        }
    }

    [SerializeField]
    public List<Event> events;

    private Dictionary<string, bool> living = new Dictionary<string, bool>();

    private UnityEngine.AsyncOperation sceneLoad;

    // Use this for initialization
    void Start () {
		foreach(Event e in events)
        {
            e.UpdateEvent();
        }
	}
	
	// Update is called once per frame
	void Update () {
        for(int i = 0; i < events.Count; i++)
        {
            events[i].CheckEvent();
            if(events[i].delayTimer >= 0 && events[i].reset)
            {
                events[i].delayTimer -= Time.deltaTime;
            }
            if(events[i].delayTimer < 0 && events[i].reset)
            {
                events[events[i].subsequentEvent].previousFired = true;
                events[i].reset = false;
            }
        }
	}

    //Utility functions for conditions or predicates should be defined here.
    public bool IsObjectAlive(string name)
    {
        if (GameObject.Find(name) != null)
            return true;
        return false;
    }

    public bool DummyTrue()
    {
        return true;
    }

    public float NumObjectsWithTagAlive(string tag)
    {
        return GameObject.FindGameObjectsWithTag(tag).Length;
    }

    public bool IsObjectWithTagAlive(string tag)
    {
        if (GameObject.FindGameObjectWithTag(tag) != null)
            return true;
        return false;
    }

    public bool OnObjectDie(string name)
    {
        if(living.ContainsKey(name) && living[name] == true && !IsObjectAlive(name))
        {
            living[name] = false;
            return true;
        }
        else if(!living.ContainsKey(name))
        {
            living[name] = IsObjectAlive(name);
        }
        return false;
    }

    public bool OnMultipleObjectsDie(params string[] names)
    {
        bool state = false;
        foreach(string name in names)
        {
            if (OnObjectDie(name))
                state = true;
        }
        foreach(string name in names)
        {
            if (IsObjectAlive(name))
                state = false;
        }
        return state;
    }

    public bool OnAnyObjectDie(params string[] names)
    {
        bool state = false;
        foreach (string name in names)
        {
            if (OnObjectDie(name))
                state = true;
        }
        return state;
    }

    static public void TransitionLevel(string levelName)
    {
        SceneManager.LoadScene(levelName);
    }

    public void TransitionLevelDelayed(string levelName)
    {
        sceneLoad = SceneManager.LoadSceneAsync(levelName);
        sceneLoad.allowSceneActivation = false;
    }

    public void AllowLevelTransition()
    {
        if(sceneLoad != null)
            sceneLoad.allowSceneActivation = true;
    }

    public void ResetLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public bool OnButtonUp(string buttonName)
    {
        return Input.GetButtonUp(buttonName);
    }

    public void Dummy()
    {

    }
}

[System.Serializable]
public class FunctionCall : ISerializationCallbackReceiver
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
    //Should consist of the fully qualified name(i.e. System.Int32 of a variable, followed by a #, followed by the value
    public string[] paramStrings;

    internal MethodInfo info;
    internal FieldInfo field;
    internal PropertyInfo prop;

    internal GameObject FunctionCallTarget;
    internal bool active = false;

    public FunctionCall(GameObject refer, string comp, string value, bool method, object[] param)
    {
        reference = refer;
        component = comp;
        valueName = value;
        isMethod = method;
        if (param != null)
            parameters = (object[])param.Clone();
        else
            parameters = null;
        //UpdateFunctionCall();
    }

    public FunctionCall(GameObject refer, string comp, string value,  string FunctionCall, string targetName) : this(refer, comp, value, false, null)
    {

    }

    public FunctionCall() : this(null, "", "", false, null)
    {

    }

    public FunctionCall(FunctionCall w)
    {
        reference = w.reference;
        component = w.component;
        valueName = w.valueName;
        isMethod = w.isMethod;
        if (w.parameters != null)
            parameters = (object[])w.parameters.Clone();
        else
            parameters = null;
    }

    public void UpdateFunctionCall()
    {
        System.Type type;
        if (component == "GameObject")
            type = reference.GetType();
        else
        {
            UnityEngine.Component cp = reference.GetComponent(component);
            type = cp.GetType();
        }
        if (isMethod)
            info = type.GetMethod(valueName);
        else if ((prop = type.GetProperty(valueName)) == null)
            field = type.GetField(valueName);
    }

    //TODO: setting paramStrings here causes editor to not let me edit paramStrings
    public void OnBeforeSerialize()
    {
        List<string> temp = new List<string>();
        foreach(object o in parameters)
        {
            string sParam = o.GetType().ToString() + "#" + o.ToString();
            temp.Add(sParam);
        }
        //paramStrings = temp.ToArray();
    }
    
    public void OnAfterDeserialize()
    {
        List<object> temp = new List<object>();
        if (paramStrings != null)
        {
            foreach (string s in paramStrings)
            {
                string[] substr = s.Split('#');
                Type t = Type.GetType(substr[0]);
                if(t != null)
                    temp.Add(TypeDescriptor.GetConverter(t).ConvertFromString(substr[1]));
            }
        }
        parameters = temp.ToArray();
    }
}


