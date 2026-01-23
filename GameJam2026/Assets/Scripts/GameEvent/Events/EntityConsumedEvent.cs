using GameEvent.Events;
using UnityEngine;

public struct EntityConsumedEvent : IEvent
{
    public GameObject Consumer;
    public GameObject Consumed;

    public EntityConsumedEvent(GameObject consumer, GameObject consumed)
    {
        Consumer = consumer;
        Consumed = consumed;
    }
}