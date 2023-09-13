using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Collections;
using Cysharp.Threading.Tasks;
using Unity.Netcode;
using MonsterLove.StateMachine;
using System;

/// <summary>
/// The game runner is responsible for running the game. It will be run on the server.
/// </summary>
public class BaseGameRunner: NetworkBehaviour{
    protected Dictionary<ulong, IPlayer> PlayerMap;
    public BaseLocalPlayerManager localPlayerManager;
    public event Action OnAllPlayerSpawned;
    void Awake()
    {
    }
    public async void RunGame(){
        if (IsServer){
            GameInitialize();
            await LoadPlayer();

            PreGameStart();
            GameStart();
        }
    } 
    /// <summary>
    /// Initialize the game. This method will be invoked on the server. Runs before all players are loaded.
    /// </summary>
    protected virtual void GameInitialize(){
        
    }
    /// <summary>
    /// Server loads all players.
    /// </summary>
    /// <returns></returns>
    private async UniTask LoadPlayer(){
        var playerSpawner = new PlayerSpawner();
        PlayerMap = playerSpawner.SpawnAllPlayers();
        OnAllPlayerSpawned?.Invoke();
        await UniTask.WaitUntil(() => PlayerMap.Values.All(player => player.IsLoaded));
    }

    protected virtual void PreGameStart(){

    }

    protected virtual void GameStart(){

    }
}
