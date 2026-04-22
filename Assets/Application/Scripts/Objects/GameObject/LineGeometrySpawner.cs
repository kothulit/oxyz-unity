using System.IO;
using UnityEngine;

public class LineGeometrySpawner : MonoBehaviour
{
    [SerializeField] private string _relativePath = "Examples/line-001.xml";
    [SerializeField] private Material _material;

    private void Start()
    {
        //string path = Path.Combine(Application.dataPath, _relativePath);
        //LineGeometry geometry = LineGeometryXmlLoader.LoadFromFile(path);
        //Mesh mesh = LineMeshBuilder.BuildMesh(geometry);
        //if (mesh == null)
        //{
        //    Debug.LogError("Failed to build mesh from geometry.");
        //    return;
        //}
        //var go = new GameObject(geometry.Id ?? "LineObject");
        //var meshFilter = go.AddComponent<MeshFilter>();
        //var meshRenderer = go.AddComponent<MeshRenderer>();
        //meshFilter.sharedMesh = mesh;

        //_material.SetFloat("_Thickness", 5f);
        //_material.SetFloat("_UseDash", 1);
        //_material.SetFloat("_DashSize", 10);
        //_material.SetFloat("_GapSize", 5);

        //meshRenderer.sharedMaterial = _material;
        //meshRenderer.material = _material;
    }
}