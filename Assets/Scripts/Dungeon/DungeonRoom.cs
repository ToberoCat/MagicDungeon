using Enemy;
using UnityEngine;

namespace Dungeon
{
    [CreateAssetMenu(fileName = "room", menuName = "Dungeon/Room", order = 1)]
    public class DungeonRoom : ScriptableObject
    {
        public GameObject prefab;
        [Range(0f, 1f)] public float probability;
        public DungeonOption[] options;
        public EnemyPool enemyPool;

        public void SpawnRoom(Transform parent, Vector2 worldPosition)
        {
            Instantiate(prefab, worldPosition, Quaternion.identity, parent);
        }
    }
}