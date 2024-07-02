using UnityEngine;

namespace Dungeon
{
    [CreateAssetMenu(fileName = "dungeon", menuName = "Dungeon/Settings", order = 0)]
    public class DungeonSettings : ScriptableObject
    {
        public DungeonRoom[] allRooms;
        public int roomSize;
    }
}