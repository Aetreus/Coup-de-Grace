using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


//Class for adding spec creation/loading functions to menus.
public class SpecMenu
{

    //Create a typespec from the currently selected game object if possible.
    [MenuItem("GameObject/Create Spec",false,12)]
    static void CreateSpec()
    {
        GameObject sel = Selection.activeGameObject;
        Object ret = PrefabUtility.GetPrefabParent(sel);
        if(ret != null)
        {
            GameObject prefab = (GameObject)ret;
            PropertyModification[] mods = PrefabUtility.GetPropertyModifications(sel);
            PropertyModification mod = mods[0];
        }
    }
}
