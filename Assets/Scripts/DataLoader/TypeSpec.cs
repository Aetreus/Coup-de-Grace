using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[System.Serializable]
public class TypeSpec{

    public string prefabName;

    private GameObject _prefabBase;

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
}
