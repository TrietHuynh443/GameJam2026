using GameEvent.Events;
using UnityEngine;

namespace Trigger
{
    public class TriggerEvent : IEvent
    {
        public TriggerObject Trigger { get; }
        public GameObject TriggeredObject { get; }
        public TriggerEventType EventType { get; }
        public TriggerPhase Phase { get; }

        public TriggerEvent(
            TriggerObject trigger,
            GameObject triggeredObject,
            TriggerEventType eventType,
            TriggerPhase phase
        )
        {
            Trigger = trigger;
            TriggeredObject = triggeredObject;
            EventType = eventType;
            Phase = phase;
        }
    }

    public enum TriggerEventType
    {
        None,
        ApplyMask,
        Infect,
        Drag,
        StopDrag,
        Fight,
        Cure,
    }

    public enum TriggerPhase
    {
        Enter,
        Stay,
        Exit
    }
}
