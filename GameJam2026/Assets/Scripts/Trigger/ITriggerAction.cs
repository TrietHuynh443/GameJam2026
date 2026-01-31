using UnityEngine;

namespace Trigger
{
    public interface ITriggerAction
    {
        TriggerEventType TriggerType { get; }
        void Execute(TriggerObject source, GameObject target, TriggerPhase phase);
    }
}