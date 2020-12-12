using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageCommunicator.TestGui
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
    public class TypeAliasAttribute : Attribute
    {
        public string AliasName { get; }

        public TypeAliasAttribute(string aliasName)
        {
            this.AliasName = aliasName;
        }
    }
}
