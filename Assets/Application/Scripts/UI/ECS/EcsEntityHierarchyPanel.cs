using System.Collections.Generic;
using UnityEngine;

public class EcsEntityHierarchyPanel : MonoBehaviour
{
    [SerializeField] private Transform _contentRoot;
    [SerializeField] private EntityHierarchyItemView _itemPrefab;

    private readonly List<EntityHierarchyItemView> _items = new();

    public void Rebuild(IReadOnlyList<EntityHierarchyRow> rows)
    {
        Clear();

        foreach (var row in rows)
        {
            var item = Instantiate(_itemPrefab, _contentRoot);
            item.SetData(row);
            _items.Add(item);
        }
    }

    public void Clear()
    {
        foreach (var item in _items)
        {
            if (item != null)
                Destroy(item.gameObject);
        }

        _items.Clear();
    }
}