using UnityEngine;
using Human;
using PlayerResources;
using GameEvent.Events;

public class GameManager : MonoBehaviour
{
    [Header("NPC Pool")]
    [SerializeField] private NPCStateController npcPrefab;
    [SerializeField] private int poolSize = 50;

    [Header("Spawn Count")]
    public int normalCount = 20;
    public int angryCount = 10;
    public int sickCount = 5;

    [Header("Spawn Area")]
    public Vector2 minSpawn;
    public Vector2 maxSpawn;

    private ObjectPool<NPCStateController> _npcPool;
    
    
    private PlayerScore _score;
    public float score;

    private void Awake()
    {
        _npcPool = new ObjectPool<NPCStateController>(
            npcPrefab,
            poolSize,
            transform
        );

        _score = new PlayerScore();
    }

    private void OnEnable()
    {
        GameEvent.GameEvent.Subscribe<ScoreEvent>(OnScoreEvent);
    }

    private void OnDisable()
    {
        GameEvent.GameEvent.Unsubscribe<ScoreEvent>(OnScoreEvent);
    }

    private void Start()
    {
        SpawnGroup(HumanState.Normal, normalCount);
        SpawnGroup(HumanState.Angry, angryCount);
        SpawnGroup(HumanState.Sick, sickCount);

        _score.UpdateResource(PlayerResourceChangeReason.Normal, normalCount + angryCount);
        _score.UpdateResource(PlayerResourceChangeReason.Infected, sickCount);
    }

    private void SpawnGroup(HumanState state, int count)
    {
        for (int i = 0; i < count; i++)
        {
            NPCStateController npc = _npcPool.Get();
            npc.transform.position = GetRandomPosition();
            npc.SetState(state);
        }
    }

    private Vector3 GetRandomPosition()
    {
        return new Vector3(
            Random.Range(minSpawn.x, maxSpawn.x),
            Random.Range(minSpawn.y, maxSpawn.y),
            0f
        );
    }

    private void OnScoreEvent(ScoreEvent evt)
    {
        if (evt.Masked != 0)
            _score.UpdateResource(PlayerResourceChangeReason.Masked, evt.Masked);

        if (evt.Infected != 0)
            _score.UpdateResource(PlayerResourceChangeReason.Infected, evt.Infected);

        score = _score.Amount;
    }
}
