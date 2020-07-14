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
         
        DetailSettings
    }

    public class DetailSettingsAttribute : Attribute
    {
        public DetailSettingsAttribute()
        {

        }
    }
}
