using Trigger;
using UnityEngine;

public class ApplyMaskAction : MonoBehaviour, ITriggerAction
{
    public TriggerEventType TriggerType => TriggerEventType.ApplyMask;

    public void Execute(TriggerObject source, GameObject target, TriggerPhase phase)
    {
        if (phase != TriggerPhase.Stay) return;

        GameEvent.GameEvent.Publish(new TriggerEvent(
            source,
            target,
            TriggerType,
            phase
        ));
    }
}