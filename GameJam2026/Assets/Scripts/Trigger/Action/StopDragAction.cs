using Trigger;
using UnityEngine;
public class StopDragAction : MonoBehaviour, ITriggerAction
{
    public TriggerEventType TriggerType => TriggerEventType.StopDrag;

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