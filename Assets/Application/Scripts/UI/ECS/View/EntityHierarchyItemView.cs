using TMPro;
using UnityEngine;

public class EntityHierarchyItemView : MonoBehaviour
{
    [SerializeField] private TMP_Text _label;
    public void SetData(EntityHierarchyRow row)
    {
        string indent = new string(' ', row.Depth * 4);
        _label.text = indent + row.Name;
    }
}
