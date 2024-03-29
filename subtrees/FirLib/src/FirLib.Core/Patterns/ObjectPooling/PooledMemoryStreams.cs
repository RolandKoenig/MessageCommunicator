﻿using System;
using System.Collections.Concurrent;
using System.IO;

namespace FirLib.Core.Patterns.ObjectPooling;

public class PooledMemoryStreams
{
    private ConcurrentStack<MemoryStream> _memoryStreams;

    public int Count => _memoryStreams.Count;

    public static PooledMemoryStreams Current
    {
        get;
        private set;
    }

    static PooledMemoryStreams()
    {
        Current = new PooledMemoryStreams();
    }

    public PooledMemoryStreams()
    {
        _memoryStreams = new ConcurrentStack<MemoryStream>();
    }

    public IDisposable UseMemoryStream(out MemoryStream memoryStream, int requiredCapacity = 128)
    {
        memoryStream = this.TakeMemoryStream(requiredCapacity);

        var cachedMemoryStream = memoryStream;
        return new DummyDisposable(() => this.ReRegisterMemoryStream(cachedMemoryStream));
    }

    public MemoryStream TakeMemoryStream(int requiredCapacity = 128)
    {
        if (!_memoryStreams.TryPop(out var result))
        {
            result = new MemoryStream(requiredCapacity);
        }
        else
        {
            if (result.Capacity < requiredCapacity) { result.Capacity = requiredCapacity; }
        }
        return result;
    }

    public void ReRegisterMemoryStream(MemoryStream memoryStream)
    {
        var buffer = memoryStream.GetBuffer();
        var newAroundSameBuffer = new MemoryStream(buffer, 0, buffer.Length, true, true);
        newAroundSameBuffer.SetLength(0);

        _memoryStreams.Push(newAroundSameBuffer);
    }

    public void Clear()
    {
        _memoryStreams.Clear();
    }
}