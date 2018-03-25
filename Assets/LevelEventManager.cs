using System.Collections;
using System.Reflection;
using System.Collections.Generic;
using UnityEngine;

public class LevelEventManager : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}

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
        UpdateFunctionCall();
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
        System.Type type = reference.GetComponent(component).GetType();
        if (isMethod)
            info = reference.GetType().GetMethod(valueName);
        else if ((prop = type.GetProperty(valueName)) == null)
            field = type.GetField(valueName);
    }

    public void OnBeforeSerialize() { }

    public void OnAfterDeserialize()
    {
        UpdateFunctionCall();
    }
}
