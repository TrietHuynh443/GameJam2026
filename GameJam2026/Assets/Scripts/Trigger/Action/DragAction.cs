using Trigger;
using UnityEngine;

public class DragAction : MonoBehaviour, ITriggerAction
{
    public TriggerEventType TriggerType => TriggerEventType.Drag;

    public void Execute(TriggerObject source, GameObject target, TriggerPhase phase)
    {
        if (phase != TriggerPhase.Stay)
            return;

        TriggerEvent evt = new(
            source,
            target,
            TriggerType,
            phase
        );

        GameEvent.GameEvent.Publish(evt);
    }
}

