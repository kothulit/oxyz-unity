using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
public sealed class ProjectLoader : IProjectLoader
{
    public Project Load(string projectRootPath)
    {
        if (string.IsNullOrWhiteSpace(projectRootPath))
            throw new ArgumentException("Project root path is empty.", nameof(projectRootPath));

        string root = Path.GetFullPath(projectRootPath);
        if (!Directory.Exists(root))
            throw new DirectoryNotFoundException($"Project folder not found: {root}");

        // 1) Document
        string documentPath = FindDocumentXmlPath(root);
        if (!File.Exists(documentPath))
            throw new FileNotFoundException($"Document file not found: {documentPath}");
        Document document = DeserializeFromFile<Document>(documentPath);

        // 2) Styles
        string stylesRoot = Path.Combine(root, "Styles");
        var stylesByName = LoadStyles(stylesRoot);

        // 3) Materials
        string materialsRoot = Path.Combine(root, "Material");
        var materialsByName = LoadMaterials(materialsRoot);
        
        return new Project
        {
            RootPath = root,
            Document = document,
            StylesByName = stylesByName,
            MaterialsByName = materialsByName
        };
    }

    private static string FindDocumentXmlPath(string root)
    {
        string[] rootXmlFiles = Directory.GetFiles(root, "*.xml", SearchOption.TopDirectoryOnly);
        Array.Sort(rootXmlFiles, StringComparer.OrdinalIgnoreCase);
        foreach (string file in rootXmlFiles)
        {
            try
            {
                var serializer = new System.Xml.Serialization.XmlSerializer(typeof(Document));
                using var stream = File.OpenRead(file);
                _ = (Document)serializer.Deserialize(stream);
                return file;
            }
            catch
            {
                // не Document, идем дальше
            }
        }
        throw new FileNotFoundException($"No XML file with root Document found in: {root}");
    }

    private Dictionary<string, Style> LoadStyles(string stylesRoot)
    {
        var result = new Dictionary<string, Style>(StringComparer.Ordinal);
        if (!Directory.Exists(stylesRoot))
            return result; // ƒл€ MVP можно разрешить пустую библиотеку
        string[] files = Directory.GetFiles(stylesRoot, "*.xml", SearchOption.AllDirectories);
        foreach (string file in files)
        {
            Style style = DeserializeFromFile<Style>(file);
            if (style == null)
                throw new InvalidOperationException($"Style deserialize returned null: {file}");
            if (string.IsNullOrWhiteSpace(style.Name))
                throw new InvalidOperationException($"Style has empty name: {file}");
            if (result.ContainsKey(style.Name))
                throw new InvalidOperationException($"Duplicate style name '{style.Name}' in file: {file}");
            result.Add(style.Name, style);
        }
        return result;
    }

    private Dictionary<string, Material> LoadMaterials(string materialsRoot)
    {
        var result = new Dictionary<string, Material>(StringComparer.Ordinal);
        if (!Directory.Exists(materialsRoot))
            return result; // ƒл€ MVP можно разрешить пустую библиотеку
        string[] files = Directory.GetFiles(materialsRoot, "*.xml", SearchOption.AllDirectories);
        foreach (string file in files)
        {
            Material material = DeserializeFromFile<Material>(file);
            if (material == null)
                throw new InvalidOperationException($"Material deserialize returned null: {file}");
            if (string.IsNullOrWhiteSpace(material.Name))
                throw new InvalidOperationException($"Material has empty name: {file}");
            if (result.ContainsKey(material.Name))
                throw new InvalidOperationException($"Duplicate material name '{material.Name}' in file: {file}");
            result.Add(material.Name, material);
        }
        return result;
    }
    private static T DeserializeFromFile<T>(string path)
    {
        var serializer = new XmlSerializer(typeof(T));
        using var stream = File.OpenRead(path);
        return (T)serializer.Deserialize(stream);
    }
}