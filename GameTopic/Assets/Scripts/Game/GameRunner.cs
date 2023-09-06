using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Collections;
using Cysharp.Threading.Tasks;

public class GameRunner: MonoBehaviour{
    public ConnectionManager connectionManager;
    public Transform[] SpawnPoints;
    private Dictionary<ulong, IPlayer> Players;
    public LocalPlayerManager localPlayerManager;
    private GameEffectManager gameEffectManager;
    void Start()
    {
        GameSetup();
        connectionManager.OnAllPlayerConnected += async () => {
            
            await PlayerSetup();
            GameStart();
        };
    }

    private void GameSetup(){
        gameEffectManager = new GameEffectManager();
        gameEffectManager.Enable();
    }

    private async UniTask PlayerSetup(){
        var playerSpawner = new PlayerSpawner();
        Players = playerSpawner.SpawnAllPlayers();
        await UniTask.WaitUntil(() => Players.Values.All(player => player.IsLoaded));
        SetPlayerSpawnPoints();
        LocalPlayerSetup();
    }
    private void LocalPlayerSetup(){
        localPlayerManager.LocalPlayerSetup_ClientRpc();
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