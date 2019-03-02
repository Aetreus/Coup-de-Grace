using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using System.Xml.Schema;
using UnityEngine;
using UnityEditor;
using UnityEngine.Serialization;


//Class for adding spec creation/loading functions to menus.
public class SpecMenu
{

    //Create a typespec from the currently selected game object if possible.
    [MenuItem("GameObject/Create Spec",false,12)]
    static void CreateSpec()
    {
        GameObject sel = Selection.activeGameObject;
        Object ret = PrefabUtility.GetPrefabParent(sel);
        if (ret != null)
        {
            GameObject prefab = (GameObject)ret;
            //Initialize spec with prefab path.
            TypeSpec spec = new TypeSpec() { prefabName = AssetDatabase.GetAssetPath(prefab), specName = "Default" };
            PropertyModification[] mods = PrefabUtility.GetPropertyModifications(sel);
            //Add all the changed properties to the type spec.
            foreach (PropertyModification mod in mods)
            {
                //Ignore properties that deal only with the game object or the transform
                SerializedObject so = new SerializedObject(mod.target);
                SerializedProperty prop = so.FindProperty(mod.propertyPath);
                System.Type compType  = so.targetObject.GetType();
                if (compType != typeof(UnityEngine.Transform) && compType != typeof(UnityEngine.GameObject))
                {
                    spec.AddMod(compType.ToString(), mod.propertyPath, prop.propertyType.ToString(), mod.value.ToString());
                }
            }

            //Write the type spec out to a file.
            string path = Application.streamingAssetsPath + "/Specs/" + spec.specName + ".xml";
            XmlSerializer xs = new XmlSerializer(spec.GetType());
            StreamWriter writer = new StreamWriter(path);
            xs.Serialize(writer, spec);
            writer.Close();

            //TODO:Update all objects using the same spec/having the same differences from the prefab to match, carry the spec name with them.
        }
    }
}
