using UnityEngine;
using TMPro;
using SimpleFileBrowser;
public class ProjectFolderPicker : MonoBehaviour
{
    [SerializeField] private TMP_InputField _pathInput;
    [SerializeField] private TMP_Text _statusText;
    public void PickProjectFolder()
    {
        FileBrowser.ShowLoadDialog(
            onSuccess: OnFolderSelected,
            onCancel: OnSelectionCanceled,
            pickMode: FileBrowser.PickMode.Folders,
            allowMultiSelection: false,
            initialPath: null,
            initialFilename: null,
            title: "Select Project Folder",
            loadButtonText: "Select"
        );
    }
    private void OnFolderSelected(string[] paths)
    {
        if (paths == null || paths.Length == 0)
            return;
        string selectedPath = paths[0];
        if (_pathInput != null)
            _pathInput.text = selectedPath;
        if (_statusText != null)
            _statusText.text = $"Selected: {selectedPath}";
        Debug.Log($"Selected folder: {selectedPath}");
    }
    private void OnSelectionCanceled()
    {
        if (_statusText != null)
            _statusText.text = "Folder selection canceled.";
        Debug.Log("Folder selection canceled.");
    }
}