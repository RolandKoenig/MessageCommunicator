using System;
using System.Collections.Generic;
using System.Text;

namespace MessageCommunicator.TestGui
{
    [AttributeUsage(AttributeTargets.Property)]
    public class TextAndHexadecimalEditAttribute : Attribute
    {
        public string EncodingWebNamePropertyName { get; set; }

        public TextAndHexadecimalEditAttribute(string encodingWebNameProperty)
        {
            this.EncodingWebNamePropertyName = encodingWebNameProperty;
        }
    }
}
