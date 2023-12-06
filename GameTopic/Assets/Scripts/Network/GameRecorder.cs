using UnityEngine;

public class GameRecorder : MonoBehaviour
{
    public void AddNewGameResult(GameResult result)
    {
        // print result.PlayerRankMap each item
        foreach (var item in result.PlayerRankMap)
        {
            Debug.Log(item.Key + " " + item.Value);
        }
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