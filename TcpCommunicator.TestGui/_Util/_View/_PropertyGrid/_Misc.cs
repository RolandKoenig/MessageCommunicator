using System;
using System.Collections.Generic;
using System.Text;

namespace TcpCommunicator.TestGui
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

    public class DetailSettingsAttribute : Attribute
    {
        public DetailSettingsAttribute()
        {

        }
    }
}
