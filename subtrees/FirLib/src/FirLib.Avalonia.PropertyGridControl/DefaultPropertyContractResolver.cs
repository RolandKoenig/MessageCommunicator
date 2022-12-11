namespace FirLib.Avalonia.PropertyGridControl;

public class DefaultPropertyContractResolver : IPropertyContractResolver
{
    private Dictionary<Type, Dictionary<string, List<Attribute>>> _dictTypes;

    public DefaultPropertyContractResolver()
    {
        _dictTypes = new Dictionary<Type, Dictionary<string, List<Attribute>>>();
    }

    public void AddDataAnnotation<T>(Type targetType, string propertyName, T attrib)
        where T : Attribute
    {
        var attributes = this.GetAttributes(targetType, propertyName);
        attributes.Add(attrib);
    }

    public void RemoveDataAnnotation<T>(Type targetType, string propertyName)
        where T : Attribute
    {
        var attributes = this.GetAttributes(targetType, propertyName);
        for (var loop = attributes.Count; loop > -1; loop--)
        {
            if (attributes[loop] is T)
            {
                attributes.RemoveAt(loop);
            }
        }

        if (attributes.Count == 0)
        {
            _dictTypes.Remove(targetType);
        }
    }

    public T? GetDataAnnotation<T>(Type targetType, string propertyName)
        where T : Attribute
    {
        var attributes = this.GetAttributes(targetType, propertyName);
        foreach (var actAttrib in attributes)
        {
            if (actAttrib is T foundAttrib)
            {
                return foundAttrib;
            }
        }
        return null;
    }

    public IEnumerable<Attribute> GetDataAnnotations(Type targetType, string propertyName)
    {
        return this.GetAttributes(targetType, propertyName);
    }

    private List<Attribute> GetAttributes(Type targetType, string propertyName)
    {
        if (!_dictTypes.TryGetValue(targetType, out var dictProperties))
        {
            dictProperties = new Dictionary<string, List<Attribute>>();
            _dictTypes.Add(targetType, dictProperties);
        }

        if (!dictProperties.TryGetValue(propertyName, out var listAttributes))
        {
            listAttributes = new List<Attribute>();
            dictProperties.Add(propertyName, listAttributes);
        }

        return listAttributes;
    }
}