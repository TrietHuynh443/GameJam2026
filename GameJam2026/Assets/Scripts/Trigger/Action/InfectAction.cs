using Trigger;
using UnityEngine;

public class InfectAction : MonoBehaviour, ITriggerAction
{
    public TriggerEventType TriggerType => TriggerEventType.Infect;

    public void Execute(TriggerObject source, GameObject target, TriggerPhase phase)
    {
        if (phase != TriggerPhase.Enter) return;

        TriggerEvent evt = new(
            source,
            target,
            TriggerType,
            phase
        );

        GameEvent.GameEvent.Publish(evt);
    }
}