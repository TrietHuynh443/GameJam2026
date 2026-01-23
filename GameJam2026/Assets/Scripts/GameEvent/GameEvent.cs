using System;
using System.Collections.Generic;

public static class GameEvent
{
    private static readonly Dictionary<Type, List<Delegate>> _handlers = new();

    public static void Subscribe<T>(Action<T> handler)
    {
        var type = typeof(T);

        if (!_handlers.TryGetValue(type, out var list))
        {
            list = new List<Delegate>();
            _handlers[type] = list;
        }

        if (!list.Contains(handler))
            list.Add(handler);
    }

    public static void Unsubscribe<T>(Action<T> handler)
    {
        var type = typeof(T);

        if (_handlers.TryGetValue(type, out var list))
        {
            list.Remove(handler);

            if (list.Count == 0)
                _handlers.Remove(type);
        }
    }

    public static void Publish<T>(T evt)
    {
        if (!_handlers.TryGetValue(typeof(T), out var list))
            return;

        // COPY to prevent modification during iteration
        var snapshot = list.ToArray();

        foreach (Action<T> handler in snapshot)
            handler.Invoke(evt);
    }

    public static void Clear()
    {
        _handlers.Clear();
    }
}