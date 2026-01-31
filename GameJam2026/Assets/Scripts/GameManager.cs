using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

        _total = _waves.Sum(w =>
            w.normalCount +
            w.angryCount +
            w.sickCount
        );

        _endTime = DateTime.UtcNow.AddSeconds(
            _waves.Sum(w => w.delayAfterWave)
        );

        Debug.Log($"Starting Level {CurrentLevel} with {_waves.Length} waves");

        StartCoroutine(SpawnWaves());
    }


    


    // ----------------------------
    // WAVE SYSTEM
    // ----------------------------
    private SpawnWave[] GetCurrentLevelWaves()
    {
        if (!LevelWaves.TryGetValue(CurrentLevel, out var waves))
        {
            Debug.LogError($"[GameManager] No waves defined for level {CurrentLevel}");
            return Array.Empty<SpawnWave>();
        }

        return waves;
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
    
    private static readonly Dictionary<int, SpawnWave[]> LevelWaves = new Dictionary<int, SpawnWave[]>
    {
        {
            1, new[]
            {
                new SpawnWave
                {
                    normalCount = 2,
                    angryCount = 0,
                    sickCount = 0,
                    spawnAreaIndices = new[] { 0 },
                    delayAfterWave = 10f
                },
                new SpawnWave
                {
                    normalCount = 6,
                    angryCount = 0,
                    sickCount = 2,
                    spawnAreaIndices = new[] { 0, 1 },
                    delayAfterWave = 20f
                }
            }
        },

        {
            2, new[]
            {
                new SpawnWave
                {
                    normalCount = 2,
                    angryCount = 1,
                    sickCount = 0,
                    spawnAreaIndices = new[] { 2 },
                    delayAfterWave = 12f
                },
                new SpawnWave
                {
                    normalCount = 5,
                    angryCount = 2,
                    sickCount = 1,
                    spawnAreaIndices = new[] { 0 },
                    delayAfterWave = 22f
                },
                new SpawnWave
                {
                    normalCount = 4,
                    angryCount = 3,
                    sickCount = 1,
                    spawnAreaIndices = new[] { 2 },
                    delayAfterWave = 25f
                }
            }
        },

        {
            3, new[]
            {
                new SpawnWave
                {
                    normalCount = 3,
                    angryCount = 0,
                    sickCount = 0,
                    spawnAreaIndices = new[] { 1 },
                    delayAfterWave = 10f
                },
                new SpawnWave
                {
                    normalCount = 6,
                    angryCount = 1,
                    sickCount = 1,
                    spawnAreaIndices = new[] { 1 },
                    delayAfterWave = 20f
                },
                new SpawnWave
                {
                    normalCount = 9,
                    angryCount = 2,
                    sickCount = 2,
                    spawnAreaIndices = new[] { 1 },
                    delayAfterWave = 30f
                }
            }
        },

        {
            4, new[]
            {
                new SpawnWave { normalCount = 5, angryCount = 0, sickCount = 1, spawnAreaIndices = new[] { 0 }, delayAfterWave = 15f },
                new SpawnWave { normalCount = 5, angryCount = 0, sickCount = 1, spawnAreaIndices = new[] { 1 }, delayAfterWave = 15f },
                new SpawnWave { normalCount = 5, angryCount = 0, sickCount = 1, spawnAreaIndices = new[] { 2 }, delayAfterWave = 15f },
                new SpawnWave { normalCount = 5, angryCount = 0, sickCount = 1, spawnAreaIndices = new[] { 3 }, delayAfterWave = 15f }
            }
        },

        {
            5, new[]
            {
                new SpawnWave { normalCount = 0, angryCount = 7, sickCount = 1, spawnAreaIndices = new[] { 0, 1 }, delayAfterWave = 15f },
                new SpawnWave { normalCount = 1, angryCount = 6, sickCount = 1, spawnAreaIndices = new[] { 1, 2 }, delayAfterWave = 18f },
                new SpawnWave { normalCount = 2, angryCount = 5, sickCount = 1, spawnAreaIndices = new[] { 2, 3 }, delayAfterWave = 20f },
                new SpawnWave { normalCount = 3, angryCount = 4, sickCount = 1, spawnAreaIndices = new[] { 0, 2 }, delayAfterWave = 22f },
                new SpawnWave { normalCount = 4, angryCount = 3, sickCount = 1, spawnAreaIndices = new[] { 1, 3 }, delayAfterWave = 25f }
            }
        }
    };
}

