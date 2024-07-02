using System;
using UnityEngine;

public static class Utils
{
    public static DungeonOption CounterPart(this DungeonOption option) =>
        option switch
        {
            DungeonOption.Left => DungeonOption.Right,
            DungeonOption.Right => DungeonOption.Left,
            DungeonOption.Up => DungeonOption.Down,
            DungeonOption.Down => DungeonOption.Up,
            _ => throw new ArgumentOutOfRangeException()
        };
}