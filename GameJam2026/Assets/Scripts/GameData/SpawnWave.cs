using UnityEngine;

namespace GameData
{

    [System.Serializable]
    public class SpawnWave
    {
        public int normalCount;
        public int angryCount;
        public int sickCount;

        [Tooltip("Time before next wave starts")]
        public float delayAfterWave = 5f;
    }

}