using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
[XmlRoot("Document")]
public class Document
{
    [XmlAttribute("id")]
    public string Id { get; set; } = Guid.NewGuid().ToString();
    [XmlAttribute("name")]
    public string Name { get; set; } = "NewDocument";
    [XmlIgnore]
    public string ProjectFolderPath { get; private set; }
    [XmlArray("Extrusions")]
    [XmlArrayItem("Extrusion")]
    public List<ExtrusionGeometry> Extrusions { get; set; } = new();
    public void SetProjectFolder(string projectFolderPath)
    {
        if (string.IsNullOrWhiteSpace(projectFolderPath))
            throw new ArgumentException("Project folder path is empty.", nameof(projectFolderPath));
        ProjectFolderPath = Path.GetFullPath(projectFolderPath);
    }
    public void AddExtrusion(ExtrusionGeometry geometry)
    {
        if (geometry == null)
            throw new ArgumentNullException(nameof(geometry));
        Extrusions.Add(geometry);
    }
    public bool RemoveExtrusion(string id)
    {
        if (string.IsNullOrWhiteSpace(id))
            return false;
        var geometry = Extrusions.Find(x => x.Id == id);
        if (geometry == null)
            return false;
        Extrusions.Remove(geometry);
        return true;
    }
    public ExtrusionGeometry GetExtrusion(string id)
    {
        if (string.IsNullOrWhiteSpace(id))
            return null;
        return Extrusions.Find(x => x.Id == id);
    }
}