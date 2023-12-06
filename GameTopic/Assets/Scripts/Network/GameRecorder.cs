using UnityEngine;

public class GameRecorder : MonoBehaviour
{
    public void AddNewGameResult(GameResult result)
    {
        if (result.IsRankingGame && !result.IsGameAborted && result.GetSelfRank() == 1){
            AddNewCount(result.GameMapName);
        }
    }

    private void AddNewCount(string mapName){
        GameRecord record = ResourceManager.Instance.LoadLocalGameRecord();
        if (record.PlayerWinCountMap.ContainsKey(mapName)){
            record.PlayerWinCountMap[mapName] += 1;
        }else{
            record.PlayerWinCountMap[mapName] = 1;
        }
        ResourceManager.Instance.SaveLocalGameRecord(record);
    }
}