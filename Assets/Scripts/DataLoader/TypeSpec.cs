using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[System.Serializable]
public class TypeSpec{

    public GameObject prefabBase;

    public GameObject Instantiate()
    {
        return GameObject.Instantiate(prefabBase);
    }

    public GameObject Instantiate(Vector3 location, Quaternion rotation)
    {
        return GameObject.Instantiate(prefabBase, location, rotation);
    }
    
    public GameObject Instantiate(Vector3 location, Quaternion rotation, Transform parent)
    {
        return GameObject.Instantiate(prefabBase, location, rotation, parent);
    }
}
