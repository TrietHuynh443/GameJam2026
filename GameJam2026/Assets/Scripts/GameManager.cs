using System.Collections;
using Human;
using UnityEngine;
using PlayerResources;
using GameEvent.Events;
using GameData;

public class GameManager : MonoBehaviour
{
    [Header("NPC Pool")]
    [SerializeField] private NPCStateController npcPrefab;
    [SerializeField] private int poolSize = 50;

    [Header("Waves")]
    [SerializeField] private SpawnWave[] waves;

    [Header("Spawn Area")]
    public Vector2 minSpawn;
    public Vector2 maxSpawn;
    [SerializeField] private Transform spawnContainer;

    private ObjectPool<NPCStateController> _npcPool;

    private PlayerScore _score;
    public float score;

    private void Awake()
    {
        _npcPool = new ObjectPool<NPCStateController>(
            npcPrefab,
            poolSize,
            spawnContainer
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
        StartCoroutine(SpawnWaves());
    }

    // ----------------------------
    // WAVE SYSTEM
    // ----------------------------
    private IEnumerator SpawnWaves()
    {
        for (int i = 0; i < waves.Length; i++)
        {
            Debug.Log("Spawning wave " + (i + 1));
            SpawnWave wave = waves[i];

            SpawnGroup(HumanState.Normal, wave.normalCount);
            SpawnGroup(HumanState.Angry, wave.angryCount);
            SpawnGroup(HumanState.Sick, wave.sickCount);

            // Update score/resources
            _score.UpdateResource(
                PlayerResourceChangeReason.Normal,
                wave.normalCount + wave.angryCount
            );

            _score.UpdateResource(
                PlayerResourceChangeReason.Infected,
                wave.sickCount
            );

            yield return new WaitForSeconds(wave.delayAfterWave);
        }
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