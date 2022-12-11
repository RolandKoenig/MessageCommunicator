using System;
using System.Collections.Concurrent;
using System.Text;

namespace FirLib.Core.Patterns.ObjectPooling;

public class PooledStringBuilders
{
    private ConcurrentStack<StringBuilder> _stringBuilders;

    public int Count => _stringBuilders.Count;

    public static PooledStringBuilders Current
    {
        get;
        private set;
    }

    static PooledStringBuilders()
    {
        Current = new PooledStringBuilders();
    }

    public PooledStringBuilders()
    {
        _stringBuilders = new ConcurrentStack<StringBuilder>();
    }

    public IDisposable UseStringBuilder(out StringBuilder stringBuilder, int requiredCapacity = 128)
    {
        stringBuilder = this.TakeStringBuilder(requiredCapacity);

        var cachedStringBuilder = stringBuilder;
        return new DummyDisposable(() => this.ReRegisterStringBuilder(cachedStringBuilder));
    }

    public StringBuilder TakeStringBuilder(int requiredCapacity = 128)
    {
        if (!_stringBuilders.TryPop(out var result))
        {
            result = new StringBuilder(requiredCapacity);
        }
        else
        {
            if (result.Capacity < requiredCapacity) { result.EnsureCapacity(requiredCapacity); }
        }
        return result;
    }

    public void ReRegisterStringBuilder(StringBuilder stringBuilder)
    {
        stringBuilder.Remove(0, stringBuilder.Length);
        _stringBuilders.Push(stringBuilder);
    }

    public void Clear()
    {
        _stringBuilders.Clear();
    }
}