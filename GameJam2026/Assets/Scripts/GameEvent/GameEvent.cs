using System;
using System.Collections.Generic;
using GameEvent.Events;

namespace GameEvent
{
    public static class GameEvent
    {
        private static readonly Dictionary<Type, List<Delegate>> _handlers = new();

        public static void Subscribe<T>(Action<T> handler) where T : IEvent
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

        public static void Unsubscribe<T>(Action<T> handler) where T : IEvent
        {
            var type = typeof(T);

            if (_handlers.TryGetValue(type, out var list))
            {
                list.Remove(handler);

                if (list.Count == 0)
                    _handlers.Remove(type);
            }
        }

        public static void Publish<T>(T evt) where T : IEvent
        {
            if (!_handlers.TryGetValue(typeof(T), out var list))
                return;

            // COPY to prevent modification during iteration
            var snapshot = list.ToArray();

            foreach (var handler in snapshot)
                handler.DynamicInvoke(evt);
        }

        public static void Clear()
        {
            _handlers.Clear();
        }
    }
}
