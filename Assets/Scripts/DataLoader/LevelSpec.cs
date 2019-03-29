using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using UnityEngine;

public class LevelSpec : IXmlSerializable {

    private class LevelObject :IXmlSerializable
    {
        public string specName;
        public Vector3 location;
        public Quaternion rotation;
        public Vector3 scale;
        public List<TypeSpec.Delta> specificChanges = null;

        public XmlSchema GetSchema()
        {
            throw new System.NotImplementedException();
        }

        public void ReadXml(XmlReader reader)
        {
            specName = reader.GetAttribute("SpecName");
            reader.ReadStartElement();
            if (reader.Name == "Location")
            {
                location.x = System.Convert.ToSingle(reader.GetAttribute("x"));
                location.y = System.Convert.ToSingle(reader.GetAttribute("y"));
                location.z = System.Convert.ToSingle(reader.GetAttribute("z"));
            }
            reader.ReadEndElement();
            reader.ReadStartElement();
            if (reader.Name == "Rotation")
            {
                rotation.x = System.Convert.ToSingle(reader.GetAttribute("x"));
                rotation.y = System.Convert.ToSingle(reader.GetAttribute("y"));
                rotation.z = System.Convert.ToSingle(reader.GetAttribute("z"));
                rotation.w = System.Convert.ToSingle(reader.GetAttribute("w"));
            }
            reader.ReadEndElement();
            reader.ReadStartElement();
            if (reader.Name == "Scale")
            {
                scale.x = System.Convert.ToSingle(reader.GetAttribute("x"));
                scale.y = System.Convert.ToSingle(reader.GetAttribute("y"));
                scale.z = System.Convert.ToSingle(reader.GetAttribute("z"));
            }
            reader.ReadStartElement();
            if(reader.Name == "Properties")
            {
                reader.ReadStartElement();
                XmlSerializer xs = new XmlSerializer(typeof(List<TypeSpec.Delta>));
                specificChanges = (List<TypeSpec.Delta>)xs.Deserialize(reader);
                reader.ReadEndElement();
            }
            reader.ReadEndElement();

        }

        public void WriteXml(XmlWriter writer)
        {
            writer.WriteStartElement("Location");
            writer.WriteAttributeString("x", location.x.ToString());
            writer.WriteAttributeString("y", location.y.ToString());
            writer.WriteAttributeString("z", location.z.ToString());
            writer.WriteEndElement();

            writer.WriteStartElement("Rotation");
            writer.WriteAttributeString("x", rotation.x.ToString());
            writer.WriteAttributeString("y", rotation.y.ToString());
            writer.WriteAttributeString("z", rotation.z.ToString());
            writer.WriteAttributeString("w", rotation.w.ToString());
            writer.WriteEndElement();

            writer.WriteStartElement("Scale");
            writer.WriteAttributeString("x", scale.x.ToString());
            writer.WriteAttributeString("y", scale.y.ToString());
            writer.WriteAttributeString("z", scale.z.ToString());
            writer.WriteEndElement();
        }
    }

    public string levelName;

    public string sceneName;

    private List<LevelObject> objects;
    private LevelEventManager manager;

    public XmlSchema GetSchema()
    {
        throw new System.NotImplementedException();
    }

    public void ReadXml(XmlReader reader)
    {
        levelName = reader.GetAttribute("LevelName");
        sceneName = reader.GetAttribute("SceneName");

        reader.ReadStartElement();
        XmlSerializer xs = new XmlSerializer(typeof(List<LevelObject>));
        objects = (List<LevelObject>)xs.Deserialize(reader);
        reader.ReadEndElement();

    }

    public void WriteXml(XmlWriter writer)
    {
        writer.WriteAttributeString("LevelName", levelName);
        writer.WriteAttributeString("SceneName", sceneName);

        writer.WriteStartElement("LevelObjects");
        XmlSerializer xs = new XmlSerializer(typeof(List<LevelObject>));
        xs.Serialize(writer, objects);
        writer.WriteEndElement();
    }
}
