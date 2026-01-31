using UnityEngine;

namespace GameData
{
    [System.Serializable]
    public class SpawnWave
    {
        [Header("Counts")]
        public int normalCount;
        public int angryCount;
        public int sickCount;

        [Header("Spawn Areas")]
        [Tooltip("Indices of spawn areas this wave can use")]
        public int[] spawnAreaIndices;

        [Tooltip("Time before next wave starts")]
        public float delayAfterWave = 5f;
    }
}
