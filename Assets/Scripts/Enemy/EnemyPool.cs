using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Enemy
{
    [CreateAssetMenu(fileName = "pool", menuName = "Dungeon/Enemy/Pool", order = 2)]
    public class EnemyPool : ScriptableObject
    {
        public EnemyPoolEntry[] enemies;
        public float radius = 5;
        public int spawnCycles = 5;

        public void SpawnEnemies(Vector2 center)
        {
            for (var i = 0; i < spawnCycles; i++)
            {
                foreach (var enemy in enemies)
                {
                    if (!(Random.value <= enemy.probability))
                        continue;

                    var position = center + Random.insideUnitCircle * radius;
                    Instantiate(enemy.prefab, position, Quaternion.identity);
                }
            }
        }
    }

    [Serializable]
    public struct EnemyPoolEntry
    {
        public GameObject prefab;
        [Range(0f, 1f)] public float probability;
    }
}