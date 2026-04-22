using System.IO;
using UnityEngine;

public class ExtrusionGeometrySpawner : MonoBehaviour
{
    [SerializeField] private string relativePath = "Examples/space-001.xml";
    [SerializeField] private Material material;

    private void Start()
    {
        //string path = Path.Combine(Application.dataPath, relativePath);
        //ExtrusionGeometry geometry = ExtrusionGeometryXmlLoader.LoadFromFile(path);
        //Mesh mesh = ExtrusionMeshBuilder.BuildMesh(geometry);
        //if (mesh == null)
        //{
        //    Debug.LogError("Failed to build mesh from geometry.");
        //    return;
        //}
        //var go = new GameObject(geometry.Id.ToString() ?? "ExtrusionObject");
        //var meshFilter = go.AddComponent<MeshFilter>();
        //var meshRenderer = go.AddComponent<MeshRenderer>();
        //meshFilter.sharedMesh = mesh;
        //meshRenderer.sharedMaterial = material;
    }
}