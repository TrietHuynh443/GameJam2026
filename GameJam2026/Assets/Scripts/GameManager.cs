using System;
using System.Collections;
using System.Linq;
using Cysharp.Threading.Tasks;
using Human;
using UnityEngine;
using PlayerResources;
using GameEvent.Events;
using GameData;
using SceneManagement;
using TMPro;
using Random = UnityEngine.Random;


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

    [SerializeField] private TextMeshProUGUI _timer;

    [SerializeField] private TextMeshProUGUI _score;

    private DateTime _endTime;
    private ObjectPool<NPCStateController> _npcPool;
    private int _total;
    public static bool IsWin { get; set; } = false;

    private void Awake()
    {
        _npcPool = new ObjectPool<NPCStateController>(
            npcPrefab,
            poolSize,
            spawnContainer
        );

    }
    

    private void Start()
    {
        _total = waves.Sum(wave => wave.angryCount + wave.normalCount + wave.sickCount);
        _endTime = DateTime.UtcNow.AddSeconds(waves.Sum(wave => wave.delayAfterWave));
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
            

            GameEvent.GameEvent.Publish<ScoreEvent>(new ScoreEvent()
            {
                Masked = 0,
                Normal = wave.normalCount + wave.angryCount,
                Infected = wave.sickCount
            });

            yield return new WaitForSeconds(wave.delayAfterWave);
        }
        if (_total == 0)
        {
            GameManager.Result = 100;
            GameManager.IsWin = true;
        }
        else
        {
            GameManager.Result = (float)PlayerResourcesManager.Instance.Get<PlayerScore>().Normal / _total;
            GameManager.IsWin = GameManager.Result >= 0.5f;
        }

        SceneLoader.Instance.ChangeScene(EScene.End).Forget();
    }

    public static float Result { get; set; }

    private void FixedUpdate()
    {
        if(!_score || !_timer) return;
        
        _score.text = $"{PlayerResourcesManager.Instance.Get<PlayerScore>().Normal}/{_total}";
        var span = (_endTime - DateTime.UtcNow);
        _timer.text = $"{span.Minutes:00}:{span.Seconds:00}";
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