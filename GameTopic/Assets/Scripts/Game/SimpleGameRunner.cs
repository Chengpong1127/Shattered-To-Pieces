using UnityEngine;

public class SimpleGameRunner: FightGameRunner{
    public Transform[] SpawnPoints;
    private void Gaming_Enter(){
        SetPlayerSpawnPoints();
    }

    private void SetPlayerSpawnPoints(){
        int i = 0;
        foreach (var player in PlayerMap)
        {
            player.Value.SetPlayerPoint(SpawnPoints[i++]);
        }
    }
}