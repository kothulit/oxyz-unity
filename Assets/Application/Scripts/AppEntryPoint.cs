using UnityEngine;

public class AppEntryPoint : MonoBehaviour
{
    private void Awake()
    {
        Debug.Log("[AppEntryPoint] Awake");
    }

    private void Start()
    {
        Initialize();
    }

    private void Initialize()
    {
        Debug.Log("[AppEntryPoint] Initialize complete");
    }
}