using UnityEngine;

public class TopMenuController : MonoBehaviour
{
    [SerializeField] private GameObject _fileMenuPanel;
    public void ToggleFileMenu()
    {
        if (_fileMenuPanel == null)
            return;
        _fileMenuPanel.SetActive(!_fileMenuPanel.activeSelf);
    }

    public void CloseFileMenu()
    {
        if (_fileMenuPanel == null)
            return;
        _fileMenuPanel.SetActive(false);
    }
}