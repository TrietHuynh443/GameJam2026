using GameEvent.Events;
using UnityCommunity.UnitySingleton;
using UnityEngine;

namespace GameEvent
{
    [DefaultExecutionOrder(-50)]
    public class EventManager : MonoSingleton<EventManager>
    {
        // ----------------------------------
        // Lifecycle
        // ----------------------------------
        protected override void OnInitialized()
        {
            // Optional: debug hook, metrics, etc.
            // Debug.Log("EventManager initialized");
        }

        public override void ClearSingleton()
        {
            GameEvent.Clear();
        }

        // ----------------------------------
        // Raise
        // ----------------------------------
        public static void Raise<T>(T evt) where T : IEvent
        {
            // Touch Instance to guarantee creation
            _ = Instance;

            GameEvent.Publish(evt);

            // Optional debug hook
            OnEventRaised?.Invoke(evt);
        }

        public static void Raise<T>(params object[] args) where T : IEvent
        {
            _ = Instance;

            var evt = (T)System.Activator.CreateInstance(typeof(T), args);
            Raise(evt);
        }

        // ----------------------------------
        // Subscribe
        // ----------------------------------
        public static void Subscribe<T>(System.Action<T> handler)
            where T : IEvent
        {
            _ = Instance;
            GameEvent.Subscribe(handler);
        }

        public static void Unsubscribe<T>(System.Action<T> handler)
            where T : IEvent
        {
            GameEvent.Unsubscribe(handler);
        }

        // ----------------------------------
        // Debug / Tools
        // ----------------------------------
        public static System.Action<IEvent> OnEventRaised;

#if UNITY_EDITOR
        [UnityEditor.InitializeOnEnterPlayMode]
        private static void OnEnterPlayMode()
        {
            GameEvent.Clear();
        }
#endif
    }
}