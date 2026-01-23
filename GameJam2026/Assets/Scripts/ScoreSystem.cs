using UnityEngine;

public class ScoreSystem : MonoBehaviour
{
    public int Score { get; private set; }

    private void OnEnable()
    {
        GameEvent.GameEvent.Subscribe<EntityConsumedEvent>(ScorePoint);
    }

    private void OnDisable()
    {
        GameEvent.GameEvent.Unsubscribe<EntityConsumedEvent>(ScorePoint);
    }

    private void ScorePoint(EntityConsumedEvent evt)
    {
        // Rules can grow later
        Score += 1;
        Debug.Log($"Score: {Score}");
    }
}