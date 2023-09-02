using System.Collections.Generic;
using UnityEngine;


public class TwoPlayerRoomRunner: MonoBehaviour{
    public ConnectionManager connectionManager;
    private Dictionary<ulong, IPlayer> Players;
    void Start()
    {
        connectionManager.OnAllPlayerConnected += GameSetup;
    }

    private void GameSetup(){
        Debug.Log("Game Setup");
        var playerSpawner = new PlayerSpawner();
        Players = playerSpawner.SpawnAllPlayers();
        var gameEffectManager = new GameEffectManager();
        gameEffectManager.Enable();
    }
}
