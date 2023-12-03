using DigitalRuby.SoundManagerNamespace;
using UnityEngine;

public class GameRunner : BaseGameRunner
{
    public MapManager MapManager;
    public static GameRunner ServerGameRunnerInstance { get; private set; }

    public override void RunGame()
    {
        if (ServerGameRunnerInstance != null){
            Debug.LogError("There is more than one game runner in the scene.");
        }
        ServerGameRunnerInstance = this;
        base.RunGame();
    }

    public override void GameOver(GameResult result)
    {
        base.GameOver(result);
        ServerGameRunnerInstance = null;
    }
}