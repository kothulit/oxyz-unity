using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

[XmlRoot("Document")]
public class Document
{
    [XmlAttribute("id")]
    public int Id { get; private set; }

    [XmlAttribute("guid")]
    public string GUID { get; private set; } = Guid.NewGuid().ToString();

    [XmlAttribute("name")]
    public string Name { get; set; } = "NewDocument";

    [XmlArray("Extrusions")]
    [XmlArrayItem("Extrusion")]
    public List<ExtrusionGeometry> Extrusions { get; set; } = new();

    public Document(int id)
    {
        Id = id;
    }

    public Document(int id, string name)
    {
        Id = id;
        Name = name;
    }

    public void AddExtrusion(ExtrusionGeometry geometry)
    {
        if (geometry == null)
            throw new ArgumentNullException(nameof(geometry));

        Extrusions.Add(geometry);
    }

    public bool TryRemoveExtrusion(int id)
    {
        if (id <= 0)
            return false;

        var geometry = Extrusions.Find(x => x.Id == id);
        if (geometry == null)
            return false;

        Extrusions.Remove(geometry);
        return true;
    }

    public ExtrusionGeometry GetExtrusion(int id)
    {
        if (id <= 0)
            return null;

        return Extrusions.Find(x => x.Id == id);
    }
}