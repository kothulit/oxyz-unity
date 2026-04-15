using System.IO;
using UnityEngine;

public class LineGeometrySpawner : MonoBehaviour
{
    [SerializeField] private string _relativePath = "Examples/line-001.xml";
    [SerializeField] private Material _material;

    private void Start()
    {
        string path = Path.Combine(Application.dataPath, _relativePath);
        LineGeometry geometry = LineGeometryXmlLoader.LoadFromFile(path);
        Mesh mesh = LineMeshBuilder.BuildMesh(geometry);
        if (mesh == null)
        {
            Debug.LogError("Failed to build mesh from geometry.");
            return;
        }
        var go = new GameObject(geometry.Id ?? "LineObject");
        var meshFilter = go.AddComponent<MeshFilter>();
        var meshRenderer = go.AddComponent<MeshRenderer>();
        meshFilter.sharedMesh = mesh;
        meshRenderer.sharedMaterial = _material;
    }
}