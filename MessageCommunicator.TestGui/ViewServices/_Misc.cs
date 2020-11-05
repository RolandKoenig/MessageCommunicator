using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace MessageCommunicator.TestGui.ViewServices
{
    public class ImportResult<T>
        where T : class
    {
        public List<UpdatedObjectInfo<T>> UpdatedObjects { get; } = new List<UpdatedObjectInfo<T>>();

        public List<T> NewObjects { get; } = new List<T>();
    }

    public class UpdatedObjectInfo<T>
        where T : class
    {
        public T OriginalObject { get; }

        public T NewObject { get; }

        public UpdatedObjectInfo(T originalObject, T newObject)
        {
            this.OriginalObject = originalObject;
            this.NewObject = newObject;
        }
    }

    public enum MessageBoxButtons
    {
        Ok,
        OkCancel,
        YesNo,
        YesNoCancel
    }

    public enum MessageBoxResult
    {
        Ok,
        Cancel,
        Yes,
        No
    }
}
