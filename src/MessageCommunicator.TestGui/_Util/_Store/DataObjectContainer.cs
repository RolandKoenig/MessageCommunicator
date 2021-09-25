using System;
using System.Collections.Generic;
using System.Text;

namespace MessageCommunicator.TestGui
{
    public class DataObjectContainer<T>
        where T : class
    {
        public string DataTypeName
        {
            get;
            set;
        } = string.Empty;

        public List<T> DataObjects
        {
            get;
        } = new List<T>();
    }
}
