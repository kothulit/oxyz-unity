using Oxyz.Xml.Serializable;
using System.Collections.Generic;

//Клас для реализаци каскадного наследования. Задаем контекст с данными из соответствующего
//стиля и подставляем из него значения если их нет в объекте
public class Resolver
{
    public SpaceElement ResolveSpace(
    SpaceElement instance,
    Project project,
    ResolveContext context)
    {
        string styleName = !string.IsNullOrWhiteSpace(instance.Style)
            ? instance.Style
            : context.DefaultStyleByCategory.GetValueOrDefault("Space");
        SpaceElement styleDefinition = null;
        if (!string.IsNullOrWhiteSpace(styleName)
            && project.StylesByName.TryGetValue(styleName, out Style style))
        {
            styleDefinition = style.Definition as SpaceElement;
        }
        return MergeSpace(instance, styleDefinition);
    }

    private static SpaceElement MergeSpace(SpaceElement instance, SpaceElement fallback)
    {
        if (fallback == null)
            return instance;
        return new SpaceElement
        {
            Name = instance.NameSpecified ? instance.Name : fallback.Name,
            GUID = instance.GuidSpecified ? instance.GUID : fallback.GUID,
            Style = instance.StyleSpecified ? instance.Style : fallback.Style,
            InsertX = instance.InsertXSpecified ? instance.InsertX : fallback.InsertX,
            InsertY = instance.InsertYSpecified ? instance.InsertY : fallback.InsertY,
            InsertZ = instance.InsertZSpecified ? instance.InsertZ : fallback.InsertZ,
            DividingPlanes = instance.DividingPlanes ?? fallback.DividingPlanes,
            DividingBoundaries = instance.DividingBoundaries ?? fallback.DividingBoundaries,
            Spaces = instance.Spaces ?? fallback.Spaces,
            Defaults = instance.Defaults ?? fallback.Defaults
        };
    }
}
