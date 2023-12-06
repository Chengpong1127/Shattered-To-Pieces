using System.Collections.Generic;
using UnityEngine;

public class GameRecord
{
    /// <summary>
    /// The win count of player at a map.
    /// </summary>
    public Dictionary<string, int> PlayerWinCountMap = new();
    public static GameRecord DefaultRecord(){
        var record = new GameRecord();
        return record;
    }
}