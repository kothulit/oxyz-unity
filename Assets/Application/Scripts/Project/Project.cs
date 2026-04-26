using System.Collections.Generic;
using Oxyz.Xml.Serializable;

public class Project
{
    public string RootPath { get ; set; }
    public Document Document {  get; set; }

    public Dictionary<string, Style> StylesByName { get; set; }
    public Dictionary<string, Material> MaterialsByName { get; set; }
}
