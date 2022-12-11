using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace FirLib.Formats.Gpx;

public class GpxExtensions
{
    [XmlElement("Extension")]
    public List<object> Extensions { get; } = new();

    [XmlAnyElement]
    public XmlElement[]? UnknownElements { get; set; }

    [XmlAnyAttribute]
    public XmlAttribute[]? UnknownAttributes { get; set; }

    public void SetSingleExtensionObject<T>(T? extensionToSet)
        where T : class
    {
        var replaced = false;
        for(var loop=0; loop<this.Extensions.Count; loop++)
        {
            if (this.Extensions[loop] is T)
            {
                if ((!replaced) && 
                    (extensionToSet != null))
                {
                    this.Extensions[loop] = extensionToSet;
                    replaced = true;
                }
                else
                {
                    this.Extensions.RemoveAt(loop);
                    loop--;
                }
            }
        }

        if ((!replaced) &&
            (extensionToSet != null))
        {
            this.Extensions.Add(extensionToSet);
        }
    }

    public T? TryGetSingleExtension<T>()
        where T : class
    {
        foreach (var actExtension in this.Extensions)
        {
            if (actExtension is T foundExtension)
            {
                return foundExtension;
            }
        }
        return null;
    }

    public T GetOrCreateExtension<T>()
        where T : class, new()
    {
        foreach (var actExtension in this.Extensions)
        {
            if (actExtension is T foundExtension)
            {
                return foundExtension;
            }
        }

        var newObject = new T();
        this.Extensions.Add(newObject);
        return newObject;
    }

    public IEnumerable<T> GetExtensions<T>()
        where T : class
    {
        for(var loop=0; loop<this.Extensions.Count; loop++)
        {
            if (this.Extensions[loop] is T foundExtension)
            {
                yield return foundExtension;
            }
        }
    }
}