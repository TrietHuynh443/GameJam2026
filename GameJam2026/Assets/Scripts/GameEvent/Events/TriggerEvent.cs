using GameEvent.Events;
using UnityEngine;

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
    Door,
    DamageZone,
    Dialogue,
    Checkpoint,
    Point
}

public enum TriggerPhase
{
    Enter,
    Stay,
    Exit
}