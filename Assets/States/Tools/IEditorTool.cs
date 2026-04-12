using UnityEngine;

public interface IEditorTool
{
    void Activate();
    void Apply();
    void Cancel();
    void Tick();
}
