using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class ExtrusionBuildCommand : MonoBehaviour
{
    [Header("State source")]
    [SerializeField] private AppStateController _appStateController;

    [Header("Scene references")]
    [SerializeField] private Camera _sceneCamera;
    [SerializeField] private Material _material;
    [SerializeField] private Transform _generatedRoot;

    [Header("Extrusion settings")]
    [SerializeField] private float _bottomOffset = 0f;
    [SerializeField] private float _topOffset = 3f;

    private readonly List<Vector3> _pointsWorld = new();

    private bool _isBuilding;
    private int _objectCounter = 1;

    public void BeginBuild()
    {
        _pointsWorld.Clear();
        _isBuilding = true;
        Debug.Log("Extrusion build started. LMB - add points, Enter - finish, Esc - cancel.");

        if (_appStateController != null)
        {
            _appStateController.State.OnNext(AppState.Creating);
        }
    }

    private void Update()
    {
        if (!_isBuilding)
            return;
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            CancelBuild();
            return;
        }
        if (Keyboard.current.enterKey.wasPressedThisFrame || Keyboard.current.numpadEnterKey.wasPressedThisFrame)
        {
            FinishBuild();
            return;
        }
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
                return;
            if (TryGetPointOnBuildPlane(out Vector3 worldPoint))
            {
                AddPoint(worldPoint);
            }
        }
    }

    private void AddPoint(Vector3 worldPoint)
    {
        worldPoint.y = 0f;
        if (_pointsWorld.Count > 0)
        {
            Vector3 last = _pointsWorld[_pointsWorld.Count - 1];
            if (Vector3.Distance(last, worldPoint) < 0.001f)
                return;
        }
        _pointsWorld.Add(worldPoint);
        Debug.Log($"Point added: {worldPoint}");
    }

    private void FinishBuild()
    {
        if (_pointsWorld.Count < 3)
        {
            Debug.LogWarning("Need at least 3 points to create a closed contour.");
            return;
        }
        ExtrusionGeometry geometry = BuildGeometryFromPoints(_pointsWorld);
        Mesh mesh = ExtrusionMeshBuilder.BuildMesh(geometry);
        if (mesh == null)
        {
            Debug.LogError("Failed to build mesh from contour.");
            return;
        }
        CreateSceneObject(geometry, mesh);
        _pointsWorld.Clear();
        _isBuilding = false;
        Debug.Log("Extrusion build finished.");
        _appStateController.State.OnNext(AppState.Navigation);
    }

    private void CancelBuild()
    {
        _pointsWorld.Clear();
        _isBuilding = false;
        Debug.Log("Extrusion build canceled.");
        _appStateController.State.OnNext(AppState.Navigation);
    }

    private bool TryGetPointOnBuildPlane(out Vector3 worldPoint)
    {
        worldPoint = default;
        if (_sceneCamera == null)
            return false;
        Plane buildPlane = new Plane(Vector3.up, Vector3.zero);
        Vector2 screenPosition = Mouse.current.position.ReadValue();
        Ray ray = _sceneCamera.ScreenPointToRay(screenPosition);
        if (!buildPlane.Raycast(ray, out float enter))
        {
            Debug.LogError("There is no hit on the plane.");
            return false;
        }
        worldPoint = ray.GetPoint(enter);
        Debug.Log($"Hit at the point {ray.GetPoint(enter).x}, {ray.GetPoint(enter).y}");
        return true;
    }
    private ExtrusionGeometry BuildGeometryFromPoints(List<Vector3> pointsWorld)
    {
        Vector3 origin = pointsWorld[0];
        var geometry = new ExtrusionGeometry(_objectCounter++)
        {
            InsertPoint = new Point3D
            {
                X = origin.x,
                Y = 0f,
                Z = origin.z
            },
            BottomOffset = _bottomOffset,
            TopOffset = _topOffset,
            Contour = new List<LineSegment2D>()
        };
        for (int i = 0; i < pointsWorld.Count; i++)
        {
            Vector3 startWorld = pointsWorld[i];
            Vector3 endWorld = pointsWorld[(i + 1) % pointsWorld.Count];
            geometry.Contour.Add(new LineSegment2D
            {
                Start = new Point2D
                {
                    X = startWorld.x - origin.x,
                    Y = startWorld.z - origin.z
                },
                End = new Point2D
                {
                    X = endWorld.x - origin.x,
                    Y = endWorld.z - origin.z
                }
            });
        }
        return geometry;
    }
    private void CreateSceneObject(ExtrusionGeometry geometry, Mesh mesh)
    {
        var go = new GameObject(geometry.Id.ToString());
        if (_generatedRoot != null)
        {
            go.transform.SetParent(_generatedRoot, false);
        }
        var meshFilter = go.AddComponent<MeshFilter>();
        var meshRenderer = go.AddComponent<MeshRenderer>();
        meshFilter.sharedMesh = mesh;
        meshRenderer.sharedMaterial = _material;
    }
}