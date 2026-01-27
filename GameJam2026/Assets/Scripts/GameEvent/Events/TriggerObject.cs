using GameEvent;
using GameEvent.Events;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class TriggerObject : MonoBehaviour
{
    public TriggerEventType triggerType;

    private int triggeredLayer;

    private void Awake()
    {
        triggeredLayer = LayerMask.NameToLayer("Triggered");
    }

    private void Reset()
    {
        GetComponent<Collider2D>().isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        RaiseEvent(other, TriggerPhase.Enter);
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        RaiseEvent(other, TriggerPhase.Stay);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        RaiseEvent(other, TriggerPhase.Exit);
    }

    private void RaiseEvent(Collider2D other, TriggerPhase phase)
    {
        if (other.gameObject.layer != triggeredLayer)
            return;

        EventManager.Raise(
            new TriggerEvent(
                this,
                other.gameObject,
                triggerType,
                phase
            )
        );
    }
}