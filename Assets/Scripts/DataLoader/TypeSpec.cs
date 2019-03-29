using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;
using System.Xml.Schema;
using UnityEngine;
using System.Collections.ObjectModel;
using System;
using System.Reflection;
using System.ComponentModel;

[System.Serializable]
public class TypeSpec : IXmlSerializable {

    [System.Serializable]
    public class Delta
    {
        public string target;
        public string propertyPath;
        public string type;
        public string value;
    }

    public string prefabName;

    public string specName;

    private GameObject _prefabBase;

    private List<Delta> _modifications;

    private ReadOnlyCollection<Delta> _externalMods;

    public TypeSpec()
    {
        _modifications = new List<Delta>();
        _externalMods = new ReadOnlyCollection<Delta>(_modifications);
    }

    public GameObject Instantiate()
    {
        GameObject obj =  GameObject.Instantiate(_prefabBase);
        ApplySpec(obj);
        return obj;
    }

    public GameObject Instantiate(Vector3 location, Quaternion rotation)
    {
        GameObject obj = GameObject.Instantiate(_prefabBase, location, rotation);
        ApplySpec(obj);
        return obj;
    }

    public GameObject Instantiate(Vector3 location, Quaternion rotation, Transform parent)
    {
        GameObject obj = GameObject.Instantiate(_prefabBase, location, rotation, parent);
        ApplySpec(obj);
        return obj;
    }

    public void AddDelta(string target, string propertyPath, string type, string value)
    {
        _modifications.Add(new Delta() { target = target, propertyPath = propertyPath, type = type, value = value });
    }

    public void ClearDeltas()
    {
        _modifications.Clear();
    }

    public ReadOnlyCollection<Delta> GetDeltas()
    {
        return _externalMods;
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
            XmlSerializer xs = new XmlSerializer(typeof(List<Delta>));
            _modifications = (List<Delta>)xs.Deserialize(reader);
            reader.ReadEndElement();
        }
        reader.ReadEndElement();
    }

    public void WriteXml(XmlWriter writer)
    {
        writer.WriteAttributeString("Name", specName);
        writer.WriteAttributeString("Prefab", prefabName);

        writer.WriteStartElement("Properties");
        XmlSerializer xs = new XmlSerializer(typeof(List<Delta>));
        xs.Serialize(writer, _modifications);
        writer.WriteEndElement();

    }

    private void ApplySpec(GameObject obj)
    {
        foreach (Delta d in _modifications)
        {
            UnityEngine.Component c = obj.GetComponent(d.target);
            Type type = c.GetType();
            PropertyInfo prop = type.GetProperty(d.propertyPath);
            prop.SetValue(c, TypeDescriptor.GetConverter(type).ConvertFromString(d.value), null);
        }
        obj.AddComponent(typeof(TypeInfo));
        obj.GetComponent<TypeInfo>().specName = specName;
    }
}
