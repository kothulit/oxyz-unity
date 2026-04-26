public readonly struct EntityHierarchyRow
{
    public readonly int Entity;
    public readonly string Name;
    public readonly int Depth;
    public EntityHierarchyRow(int entity, string name, int depth)
    {
        Entity = entity;
        Name = name;
        Depth = depth;
    }
}