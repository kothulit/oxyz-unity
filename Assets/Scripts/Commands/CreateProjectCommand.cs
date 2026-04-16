using System;
using System.IO;
using TMPro;
using UnityEngine;

public class CreateProjectCommand : MonoBehaviour
{
    [SerializeField] private TMP_InputField _pathInput;
    [SerializeField] private TMP_Text _statusLabel;
    public void Execute()
    {
        if (_pathInput == null)
        {
            SetStatus("Path input is not assigned.");
            return;
        }
        string path = _pathInput.text?.Trim();
        if (string.IsNullOrWhiteSpace(path))
        {
            SetStatus("Project path is empty.");
            return;
        }
        try
        {
            string fullPath = Path.GetFullPath(path);
            if (Directory.Exists(fullPath))
            {
                SetStatus($"Folder already exists: {fullPath}");
                return;
            }
            Directory.CreateDirectory(fullPath);
            SetStatus($"Folder created: {fullPath}");
        }
        catch (Exception ex)
        {
            SetStatus($"Failed to create folder: {ex.Message}");
        }
    }
    private void SetStatus(string message)
    {
        Debug.Log(message);
        if (_statusLabel != null)
        {
            _statusLabel.text = message;
        }
    }
}
