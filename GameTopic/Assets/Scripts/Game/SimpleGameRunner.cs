using UnityEngine;

public class SimpleGameRunner: FightGameRunner{
    public Transform[] SpawnPoints;
    private int playerCount = 0;
    public override void SpawnDevice(BasePlayer player, string filename)
    {
        player.ServerLoadDevice(filename, SpawnPoints[playerCount++].position);
    }
}