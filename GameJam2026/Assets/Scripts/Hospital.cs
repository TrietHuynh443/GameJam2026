using Trigger;
using UnityEngine;

public class Hospital : TriggerObject
{
    protected override void OnEnable()
    {
        base.OnEnable();
    }
    
    protected override void OnTriggerEnter2D(Collider2D other)
    {
        ExecuteActions(other, TriggerPhase.Enter, TriggerEventType.Cure);
    }
}