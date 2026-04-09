using System.IO;
using UnityEngine;

public class ExtrusionBuildCommand : MonoBehaviour
{
    [SerializeField] private string _relativePath = "Objects/Examples/space-001.xml";
    [SerializeField] private Material _material;
    [SerializeField] private Transform _generatedRoot;
    public void BuildFromXml()
    {
        string path = Path.Combine(Application.dataPath, _relativePath);
        ExtrusionGeometry geometry = ExtrusionGeometryXmlLoader.LoadFromFile(path);
        Mesh mesh = ExtrusionMeshBuilder.BuildMesh(geometry);
        if (geometry == null || mesh == null)
        {
            Debug.LogError("Failed to build extrusion geometry.");
            return;
        }
        Transform parent = _generatedRoot != null ? _generatedRoot : null;
        GameObject existing = GameObject.Find(geometry.Id);
        if (existing != null)
        {
            Object.Destroy(existing);
        }
        var go = new GameObject(geometry.Id);
        if (parent != null)
        {
            go.transform.SetParent(parent, false);
        }
        var meshFilter = go.AddComponent<MeshFilter>();
        var meshRenderer = go.AddComponent<MeshRenderer>();
        meshFilter.sharedMesh = mesh;
        meshRenderer.sharedMaterial = _material;
    }
}