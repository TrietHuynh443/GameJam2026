using UnityEngine;

public class ScoreSystem : MonoBehaviour
{
    public int Score;

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
        Score += 1;
    }
}