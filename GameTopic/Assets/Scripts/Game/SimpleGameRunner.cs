using UnityEngine;

public class SimpleGameRunner: BaseGameRunner{
    private GameEffectManager gameEffectManager;
    public Transform[] SpawnPoints;
    protected override void GameInitialize(){
        
    }
    protected override void PreGameStart(){
        gameEffectManager = new GameEffectManager();
        gameEffectManager.Enable();
        SetPlayerSpawnPoints();
    }
    protected override void GameStart(){
        Debug.Log("GameStart");
    }

    private void SetPlayerSpawnPoints(){
        int i = 0;
        foreach (var player in PlayerMap)
        {
            player.Value.SetPlayerPoint(SpawnPoints[i++]);
        }
    }
}