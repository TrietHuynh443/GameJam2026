using UnityEngine;
using GameEvent.Events;

public class ScoreSystem : MonoBehaviour
{
    public int score;

    private void OnEnable()
    {
        GameEvent.GameEvent.Subscribe<ScoreEvent>(ScorePoint);
    }

    private void OnDisable()
    {
        GameEvent.GameEvent.Unsubscribe<ScoreEvent>(ScorePoint);
    }

    private void ScorePoint(ScoreEvent evt)
    {
        score += 1;
    }
}