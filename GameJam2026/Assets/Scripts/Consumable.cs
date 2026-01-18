using UnityEngine;

public class Consumable : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Consumer"))
            return;

        GameEvent.Publish(
            new EntityConsumedEvent(other.gameObject, gameObject)
        );
    }
}