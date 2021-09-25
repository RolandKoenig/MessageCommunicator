using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageCommunicator.TestGui
{
    [AttributeUsage(AttributeTargets.Property)]
    public class FixedPossibleValuesAttribute : Attribute
    {
        public string ValueGetterMethodName { get; }

        /// <summary>
        /// Creates a new <see cref="FixedPossibleValuesAttribute"/>
        /// </summary>
        /// <param name="valueGetterMethodName">The name of a static method which gets all possible values</param>
        public FixedPossibleValuesAttribute(string valueGetterMethodName)
        {
            this.ValueGetterMethodName = valueGetterMethodName;
        }
    }
}
