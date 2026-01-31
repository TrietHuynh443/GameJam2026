using UnityEngine;

namespace GameData
{
    [System.Serializable]
    public class SpawnArea
    {
        public string id;
        public Vector2 min;
        public Vector2 max;

        public Vector3 GetRandomPosition()
        {
            return new Vector3(
                Random.Range(min.x, max.x),
                Random.Range(min.y, max.y),
                0f
            );
        }
    }
}