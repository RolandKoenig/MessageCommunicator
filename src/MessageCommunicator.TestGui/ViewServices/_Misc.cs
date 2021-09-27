using System.Collections.Generic;

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
}
