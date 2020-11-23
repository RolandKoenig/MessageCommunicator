using System;
using System.Collections.Generic;
using System.Text;

namespace MessageCommunicator.TestGui
{
    public enum PropertyValueType
    {
        Bool,

        String,

        Enum,

        EncodingWebName,

        TextAndHexadecimalEdit,
         
        DetailSettings
    }

    public interface IPropertyGridContractResolver
    {
        T? GetDataAnnotation<T>(Type targetType, string propertyName)
            where T : Attribute;

        IEnumerable<Attribute> GetDataAnnotations(Type targetType, string propertyName);
    }

    public class DetailSettingsAttribute : Attribute
    {
        public DetailSettingsAttribute()
        {

        }
    }
}
