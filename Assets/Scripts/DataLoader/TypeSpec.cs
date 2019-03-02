using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;
using System.Xml.Schema;
using UnityEngine;

[System.Serializable]
public class TypeSpec : IXmlSerializable {

    [System.Serializable]
    public class Mod
    {
        public string _target;
        public string _propertyPath;
        public string _type;
        public string _value;
    }

    public string prefabName;

    public string specName;

    private GameObject _prefabBase;

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

    public XmlSchema GetSchema()
    {
        return (null);
    }

    public void ReadXml(XmlReader reader)
    {
        specName = reader.GetAttribute("Name");
        prefabName = reader.GetAttribute("Prefab");

        reader.ReadStartElement();
        if(reader.Name == "Properties")
        {
            reader.ReadStartElement();
            XmlSerializer xs = new XmlSerializer(typeof(List<Mod>));
            _modifications = (List<Mod>)xs.Deserialize(reader);
            reader.ReadEndElement();
        }
    }

    public void WriteXml(XmlWriter writer)
    {
        writer.WriteAttributeString("Name", specName);
        writer.WriteElementString("Prefab", prefabName);

        writer.WriteStartElement("Properties");
        XmlSerializer xs = new XmlSerializer(typeof(List<Mod>));
        xs.Serialize(writer, _modifications);
        writer.WriteEndElement();

    }
}
