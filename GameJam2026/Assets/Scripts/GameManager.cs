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
    public static int CurrentLevel = 1;
    public static int MaxLevel = 5;

    [Header("NPC Pool")]
    [SerializeField] private NPCStateController npcPrefab;
    [SerializeField] private int poolSize = 50;

    [Header("Waves")]
    private SpawnWave[] _waves;
    
    [Header("Levels (Mock Data)")]
    [SerializeField] private SpawnWave[] level1Waves;
    [SerializeField] private SpawnWave[] level2Waves;
    [SerializeField] private SpawnWave[] level3Waves;
    [SerializeField] private SpawnWave[] level4Waves;
    [SerializeField] private SpawnWave[] level5Waves;


    [Header("Spawn Areas")]
    [SerializeField] private SpawnArea[] spawnAreas;

    [SerializeField] private Transform spawnContainer;

    [SerializeField] private TextMeshProUGUI _timer;

    [SerializeField] private TextMeshProUGUI _score;
    [SerializeField] private TextMeshProUGUI _level;

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
        _waves = GetCurrentLevelWaves();

        _total = _waves.Sum(wave =>
            wave.angryCount +
            wave.normalCount +
            wave.sickCount
        );

        _endTime = DateTime.UtcNow.AddSeconds(
            _waves.Sum(wave => wave.delayAfterWave)
        );

        Debug.Log($"Starting Level {CurrentLevel} with {_waves.Length} waves");

        StartCoroutine(SpawnWaves());
    }

    


    // ----------------------------
    // WAVE SYSTEM
    // ----------------------------
    private SpawnWave[] GetCurrentLevelWaves()
    {
        return CurrentLevel switch
        {
            1 => level1Waves,
            2 => level2Waves,
            3 => level3Waves,
            4 => level4Waves,
            5 => level5Waves,
            _ => level5Waves // clamp to last level
        };
    }

    private IEnumerator SpawnWaves()
    {
        for (int i = 0; i < _waves.Length; i++)
        {
            SpawnWave wave = _waves[i];
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
        if(!_score || !_timer || !_level) return;
        
        _score.text = $"{PlayerResourcesManager.Instance.Get<PlayerScore>().Normal}/{_total}";
        var span = (_endTime - DateTime.UtcNow);
        _timer.text = $"{span.Minutes:00}:{span.Seconds:00}";
        _level.text = $"{CurrentLevel}";
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
    
    public static void NextLevel()
    {
        if(CurrentLevel < MaxLevel)
            CurrentLevel++;
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