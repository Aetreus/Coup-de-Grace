using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[System.Serializable]
public class TypeSpec{

    [System.Serializable]
    private class Mod
    {
        public string _target;
        public string _propertyPath;
        public string _type;
        public string _value;
    }

    public string prefabName;

    public string specName;

    [SerializeField]
    private GameObject _prefabBase;

    [SerializeField]
    private List<Mod> _modifications;

    public TypeSpec()
    {
        _modifications = new List<Mod>();
    }

    public GameObject Instantiate()
    {
        return GameObject.Instantiate(_prefabBase);
    }

    public GameObject Instantiate(Vector3 location, Quaternion rotation)
    {
        return GameObject.Instantiate(_prefabBase, location, rotation);
    }
    
    public GameObject Instantiate(Vector3 location, Quaternion rotation, Transform parent)
    {
        return GameObject.Instantiate(_prefabBase, location, rotation, parent);
    }

    public void AddMod(string target, string propertyPath, string type, string value)
    {
        _modifications.Add(new Mod() { _target = target, _propertyPath = propertyPath, _type = type, _value = value });
    }

    public void ClearMods()
    {
        _modifications.Clear();
    }
}
