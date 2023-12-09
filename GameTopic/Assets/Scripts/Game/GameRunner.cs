using System;
using UnityEngine;

public class GameRunner : BaseGameRunner
{
    public MapManager MapManager;
    public static Action<GameRunner> OnRunGame;
    public static GameRunner ServerGameRunnerInstance { get; private set; }

    public override void RunGame()
    {
        if (ServerGameRunnerInstance != null){
            Debug.LogError("There is more than one game runner in the scene.");
        }
        ServerGameRunnerInstance = this;
        base.RunGame();
        OnRunGame?.Invoke(this);
    }

    public override void GameOver(GameResult result)
    {
        base.GameOver(result);
        ServerGameRunnerInstance = null;
    }
}