using UnityEngine;

public class SimpleGameRunner: BaseGameRunner{
    private GameEffectManager gameEffectManager;
    public Transform[] SpawnPoints;
    protected override void PreGameStart(){
        SetPlayerSpawnPoints();
    }
    protected override void GameStart(){
        Debug.Log("GameStart");
        StartCoroutine(this.GetComponent<PortalSpawner>().SpawnPortal());
    }

    private void SetPlayerSpawnPoints(){
        int i = 0;
        foreach (var player in PlayerMap)
        {
            player.Value.SetPlayerPoint(SpawnPoints[i++]);
        }
    }
}