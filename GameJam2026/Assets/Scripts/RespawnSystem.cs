using UnityEngine;

public class RespawnSystem : MonoBehaviour
{
    public Vector2 minBounds;
    public Vector2 maxBounds;

    private void OnEnable()
    {
        GameEvent.Subscribe<EntityConsumedEvent>(OnConsumed);
    }

    private void OnDisable()
    {
        GameEvent.Unsubscribe<EntityConsumedEvent>(OnConsumed);
    }

    private void OnConsumed(EntityConsumedEvent evt)
    {
        evt.Consumed.transform.position = GetRandomPosition();
    }

    private Vector2 GetRandomPosition()
    {
        return new Vector2(
            Random.Range(minBounds.x, maxBounds.x),
            Random.Range(minBounds.y, maxBounds.y)
        );
    }
}