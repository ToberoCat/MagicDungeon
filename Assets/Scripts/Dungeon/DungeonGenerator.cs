using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Dungeon
{
    public class DungeonGenerator : MonoBehaviour
    {
        public static DungeonGenerator Instance { get; private set; }
        public Vector2 offset;
        [SerializeField] private DungeonSettings dungeonSettings;

        [SerializeField] private TextMeshProUGUI difficultyText;

        private readonly Dictionary<Vector2Int, DungeonRoom> _dungeonMap = new();
        private int _difficulty;

        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            GenerateDungeon();
        }

        public void GenerateDungeon()
        {
            _dungeonMap.Clear();
            _difficulty++;
            difficultyText.text = $"Diff: {_difficulty}";
            var roomOpenSet = new Queue<Vector2Int>();
            PlaceStartRoom(roomOpenSet);

            while (roomOpenSet.TryDequeue(out var nextRoom))
            {
                if (_dungeonMap.ContainsKey(nextRoom))
                    continue;

                var room = GetNextRoom(GetFilteredRooms(nextRoom), nextRoom);
                if (room.prefab == null)
                    continue;

                _dungeonMap.Add(nextRoom, room);
                GetNextToEnqueue(roomOpenSet, room, nextRoom);
            }
        }

        private void GetNextToEnqueue(Queue<Vector2Int> roomOpenSet, DungeonRoom room, Vector2Int nextRoom)
        {
            foreach (var option in room.options)
            {
                switch (option)
                {
                    case DungeonOption.Left:
                        roomOpenSet.Enqueue(nextRoom + Vector2Int.left);
                        break;
                    case DungeonOption.Right:
                        roomOpenSet.Enqueue(nextRoom + Vector2Int.right);
                        break;
                    case DungeonOption.Up:
                        roomOpenSet.Enqueue(nextRoom + Vector2Int.up);
                        break;
                    case DungeonOption.Down:
                        roomOpenSet.Enqueue(nextRoom + Vector2Int.down);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        private DungeonRoom[] GetFilteredRooms(Vector2Int location)
        {
            var openSides = new HashSet<DungeonOption>();
            var closeSides = new HashSet<DungeonOption>();

            CheckSide(Vector2Int.left, DungeonOption.Left);
            CheckSide(Vector2Int.right, DungeonOption.Right);
            CheckSide(Vector2Int.up, DungeonOption.Up);
            CheckSide(Vector2Int.down, DungeonOption.Down);


            return dungeonSettings.allRooms.Where(room => openSides.All(option => room.options.Contains(option)) &&
                                                          room.options.All(option => !closeSides.Contains(option)))
                .ToArray();

            void CheckSide(Vector2Int side, DungeonOption option)
            {
                if (!_dungeonMap.TryGetValue(location + side, out var room))
                    return;

                if (!room.options.Contains(option.CounterPart()))
                {
                    closeSides.Add(option);
                    return;
                }

                openSides.Add(option);
            }
        }

        private void PlaceStartRoom(Queue<Vector2Int> roomOpenSet)
        {
            var startRoom = GetNextRoom(dungeonSettings.allRooms, Vector2Int.zero);
            _dungeonMap.Add(Vector2Int.zero, startRoom);
            GetNextToEnqueue(roomOpenSet, startRoom, Vector2Int.zero);
        }

        private DungeonRoom GetNextRoom(DungeonRoom[] rooms, Vector2Int location)
        {
            var probabilitySum = rooms.Sum(room => room.probability);
            var randomValue = Random.value * probabilitySum;
            foreach (var room in rooms)
            {
                randomValue -= room.probability;
                if (randomValue > 0)
                    continue;

                var worldPosition =
                    new Vector2(location.x * dungeonSettings.roomSize, location.y * dungeonSettings.roomSize) + offset;
                room.SpawnRoom(transform, worldPosition);
                for (var i = 0; i < _difficulty; i++) room.enemyPool.SpawnEnemies(worldPosition - Instance.offset);
                return room;
            }

            return default;
        }
    }
}