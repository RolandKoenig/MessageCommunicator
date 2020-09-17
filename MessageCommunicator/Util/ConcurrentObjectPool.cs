using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace MessageCommunicator.Util
{
    // Original code from book "Pro .Net Memory Management" by Konrad Kokosa (sample sourcecode chapter 6)
    // Changes:
    //  - Only some namings to meet coding style of Seeing#
    //
    // Based on ObjectPool<T> from Roslyn source code (with comments reused):
    // https://github.com/dotnet/roslyn/blob/d4dab355b96955aca5b4b0ebf6282575fad78ba8/src/Dependencies/PooledObjects/ObjectPool%601.cs

    /// <summary>
    /// Helper class for reusing objects.
    /// </summary>
    internal class ConcurrentObjectPool<T> 
        where T : class
    {
        private readonly T?[] _items;
        private T? _firstItem;

        public int CountItems
        {
            get
            {
                var counter = 0;

                if (_firstItem != null) { counter++; }
                for (var loop = 0; loop < _items.Length; loop++)
                {
                    if (_items[loop] != null) { counter++; }
                }

                return counter;
            }
        }

        public ConcurrentObjectPool(int size)
        {
            _items = new T[size - 1];
        }

        public T? TryRent()
        {
            // PERF: Examine the first element. If that fails, RentSlow will look at the remaining elements.
            // Note that the initial read is optimistically not synchronized. That is intentional. 
            // We will interlock only when we have a candidate. in a worst case we may miss some
            // recently returned objects. Not a big deal.
            var inst = _firstItem;
            if (inst == null || inst != Interlocked.CompareExchange(ref _firstItem, null, inst))
            {
                inst = this.TryRentSlow();
            }
            return inst;
        }

        public bool Return(T item)
        {
            if (_firstItem == null)
            {
                // Intentionally not using interlocked here. 
                // In a worst case scenario two objects may be stored into same slot.
                // It is very unlikely to happen and will only mean that one of the objects will get collected.
                _firstItem = item;
                return true;
            }
            else
            {
                return this.ReturnSlow(item);
            }
        }

        private T? TryRentSlow()
        {
            for (var i = 0; i < _items.Length; i++)
            {
                // Note that the initial read is optimistically not synchronized. That is intentional. 
                // We will interlock only when we have a candidate. in a worst case we may miss some
                // recently returned objects. Not a big deal.
                var inst = _items[i];
                if (inst != null)
                {
                    if (inst == Interlocked.CompareExchange(ref _items[i], null, inst))
                    {
                        return inst;
                    }
                }
            }
            return null;
        }

        private bool ReturnSlow(T obj)
        {
            for (var i = 0; i < _items.Length; i++)
            {
                if (_items[i] == null)
                {
                    // Intentionally not using interlocked here. 
                    // In a worst case scenario two objects may be stored into same slot.
                    // It is very unlikely to happen and will only mean that one of the objects will get collected.
                    _items[i] = obj;
                    return true;
                }
            }
            return false;
        }
    }
}
