using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Collections;

public class TwoPlayerRoomRunner: MonoBehaviour{
    public ConnectionManager connectionManager;
    public Transform[] SpawnPoints;
    private Dictionary<ulong, IPlayer> Players;
    public LocalPlayerManager localPlayerManager;
    void Start()
    {
        connectionManager.OnAllPlayerConnected += () => {
            StartCoroutine(GameSetup());
            GameStart();
        };
    }

    private IEnumerator GameSetup(){
        Debug.Log("Game Setup");
        var playerSpawner = new PlayerSpawner();
        Players = playerSpawner.SpawnAllPlayers();
        yield return new WaitUntil(() => Players.Values.All(player => player.IsLoaded));
        SetPlayerSpawnPoints();
        SetPlayerCamera();


        var gameEffectManager = new GameEffectManager();
        gameEffectManager.Enable();
    }
    private void SetPlayerCamera(){
        localPlayerManager.SetCameraOnPlayer_ClientRpc();
    }

    private void SetPlayerSpawnPoints(){
        int i = 0;
        foreach (var player in Players)
        {
            player.Value.SetPlayerPoint(SpawnPoints[i++]);
        }
    }

    private void GameStart(){
        Debug.Log("Game Start");
    }
}
