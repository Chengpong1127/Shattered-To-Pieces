using System.Collections.Generic;
using UnityEngine;


public class TwoPlayerRoomRunner: MonoBehaviour{
    public ConnectionManager connectionManager;
    public Transform[] SpawnPoints;
    private Dictionary<ulong, IPlayer> Players;
    void Start()
    {
        connectionManager.OnAllPlayerConnected += () => {
            GameSetup();
            GameStart();
        };
    }

    private void GameSetup(){
        Debug.Log("Game Setup");
        var playerSpawner = new PlayerSpawner();
        Players = playerSpawner.SpawnAllPlayers();
        SetPlayerSpawnPoints();
        var gameEffectManager = new GameEffectManager();
        gameEffectManager.Enable();
    }

    private void SetPlayerSpawnPoints(){
        int i = 0;
        foreach (var player in Players)
        {
            player.Value.SetPlayerInitPoint(SpawnPoints[i++]);
        }
    }

    private void GameStart(){
        Debug.Log("Game Start");
    }
}
