using UnityEngine;

public struct EntityConsumedEvent
{
    public GameObject Consumer;
    public GameObject Consumed;

    public EntityConsumedEvent(GameObject consumer, GameObject consumed)
    {
        Consumer = consumer;
        Consumed = consumed;
    }
}