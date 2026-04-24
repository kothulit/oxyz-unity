using UnityEngine;
using TMPro;
using SimpleFileBrowser;
using VContainer;

public class ProjectFolderPicker : MonoBehaviour
{
    [SerializeField] private TMP_InputField _pathInput;
    [SerializeField] private TMP_Text _statusText;

    private IProjectLoader _projectLoader;
    private ProjectSession _projectSession;

    [Inject]
    public void Construct(IProjectLoader projectLoader, ProjectSession projectSession)
    {
        _projectLoader = projectLoader;
        _projectSession = projectSession;
    }

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

        try
        {
            Project project = _projectLoader.Load(selectedPath);
            _projectSession.SetCurrentProject(project);

            if (_statusText != null)
                _statusText.text = $"Loaded: {project.Document.Name} | Styles: {project.StylesByName.Count} | Materials: {project.MaterialsByName.Count}";

            Debug.Log($"Project loaded from: {selectedPath}");
        }
        catch (System.Exception ex)
        {
            if (_statusText != null)
                _statusText.text = $"Load failed: {ex.Message}";

            Debug.LogError($"Project load failed from '{selectedPath}': {ex}");
        }
    }

    private void OnSelectionCanceled()
    {
        if (_statusText != null)
            _statusText.text = "Folder selection canceled.";

        Debug.Log("Folder selection canceled.");
    }
}