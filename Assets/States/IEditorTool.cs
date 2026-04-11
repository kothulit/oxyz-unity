using UnityEngine;

public interface IEditorTool
{
    void Start();
    void Apply();
    void Cancel();
    void Tick();
}
