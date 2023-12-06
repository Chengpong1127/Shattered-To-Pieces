using UnityEngine;

public class GameRecorder : MonoBehaviour
{
    public void AddNewGameResult(GameResult result)
    {
        if (result.IsRankingGame && !result.IsGameAborted && result.GetSelfRank() == 1){
            Debug.Log("You win!");
        }
    }
}