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

    [Header("Spawn Areas")]
    [SerializeField] private SpawnArea[] spawnAreas;

    [SerializeField] private Transform spawnContainer;

    private ObjectPool<NPCStateController> _npcPool;
    private PlayerScore _score;

    private void Awake()
    {
        _npcPool = new ObjectPool<NPCStateController>(
            npcPrefab,
            poolSize,
            spawnContainer
        );

        _score = new PlayerScore();
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
            SpawnWave wave = waves[i];
            Debug.Log($"Spawning wave {i + 1}");

            SpawnGroup(HumanState.Normal, wave.normalCount, wave.spawnAreaIndices);
            SpawnGroup(HumanState.Angry, wave.angryCount, wave.spawnAreaIndices);
            SpawnGroup(HumanState.Sick, wave.sickCount, wave.spawnAreaIndices);

            _score.UpdateResource(
                PlayerResourceChangeReason.Normal,
                wave.normalCount + wave.angryCount
            );

            _score.UpdateResource(
                PlayerResourceChangeReason.Infected,
                wave.sickCount
            );

            GameEvent.GameEvent.Publish<ScoreEvent>(new ScoreEvent()
            {
                Masked = 0,
                Normal = wave.normalCount + wave.angryCount,
                Infected = wave.sickCount
            });

            yield return new WaitForSeconds(wave.delayAfterWave);
        }
    }


    private void SpawnGroup(HumanState state, int count, int[] areaIndices)
    {
        if (count <= 0 || areaIndices == null || areaIndices.Length == 0)
            return;

        for (int i = 0; i < count; i++)
        {
            NPCStateController npc = _npcPool.Get();

            SpawnArea area = GetRandomSpawnArea(areaIndices);
            npc.transform.position = area.GetRandomPosition();

            npc.SetState(state);
        }
    }

    private SpawnArea GetRandomSpawnArea(int[] areaIndices)
    {
        int index = areaIndices[Random.Range(0, areaIndices.Length)];
        return spawnAreas[Mathf.Clamp(index, 0, spawnAreas.Length - 1)];
    }
    
    
#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        if (spawnAreas == null) return;

        Gizmos.color = Color.green;

        foreach (var area in spawnAreas)
        {
            Vector2 center = (area.min + area.max) * 0.5f;
            Vector2 size = area.max - area.min;
            Gizmos.DrawWireCube(center, size);
        }
    }
#endif

}