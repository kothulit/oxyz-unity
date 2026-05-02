using Oxyz.Xml.Serializable;
using System.Collections.Generic;
using UnityEngine;

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
            Name = IsSet(instance.Name) ? instance.Name : fallback.Name,
            GUID = IsSet(instance.GUID) ? instance.GUID : fallback.GUID,
            Style = instance.Style,
            InsertX = IsSet(instance.InsertX) ? instance.InsertX : fallback.InsertX,
            InsertY = IsSet(instance.InsertY) ? instance.InsertY : fallback.InsertY,
            InsertZ = IsSet(instance.InsertZ) ? instance.InsertZ : fallback.InsertZ,
            DividingPlanes = instance.DividingPlanes ?? fallback.DividingPlanes,
            DividingBoundaries = instance.DividingBoundaries ?? fallback.DividingBoundaries,
            Spaces = instance.Spaces ?? fallback.Spaces,
            Defaults = instance.Defaults ?? fallback.Defaults
        };
    }
}
