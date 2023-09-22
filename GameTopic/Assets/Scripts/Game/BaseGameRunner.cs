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
    public StateMachine<GameStates> StateMachine;
    public IGameEventHandler[] GameEventHandlers;
    public enum GameStates{
        Initialize,
        Gaming,
        GameEnd
    }
    protected Dictionary<ulong, BasePlayer> PlayerMap;
    /// <summary>
    /// This event will be invoked when a player is spawned. The parameter is the player's network object id.
    /// </summary>
    public event Action<ulong> OnPlayerSpawned;
    void Awake()
    {
        StateMachine = new StateMachine<GameStates>(this);
        StateMachine.ChangeState(GameStates.Initialize);
        GameEventHandlers = GetComponents<IGameEventHandler>();
        GameEventHandlers.ToList().ForEach(handler => handler.enabled = false);
    }
    public async void RunGame(){
        if (IsServer){
            GameInitialize();
            await CreatePlayer();
            PreGameStart();
            StateMachine.ChangeState(GameStates.Gaming);
            GameStart();
        }
        else{
            Debug.LogError("GameRunner is not running on server");
        }
    } 
    /// <summary>
    /// Initialize the game. This method will be invoked on the server. Runs before all players are loaded.
    /// </summary>
    protected virtual void GameInitialize(){
        if(IsServer){
            GameEventHandlers.ToList().ForEach(handler => handler.enabled = true);
        }
    }
    /// <summary>
    /// Server loads all players.
    /// </summary>
    /// <returns></returns>
    private async UniTask CreatePlayer(){
        var playerSpawner = new PlayerSpawner();
        PlayerMap = playerSpawner.SpawnAllPlayers();
        PlayerMap.Values.ToList().ForEach(player => player.OnPlayerDied += () => PlayerDiedHandler(player));
        PlayerMap.Values.ToList().ForEach(player => SpawnDevice(player, "0"));
        await UniTask.WaitUntil(() => PlayerMap.Values.All(player => player.IsAlive.Value));
    }

    public virtual async void SpawnDevice(BasePlayer player, string filename){
        await player.ServerLoadDevice(filename);
        OnPlayerSpawned?.Invoke(player.OwnerClientId);
    }

    protected virtual async void PlayerDiedHandler(BasePlayer player){
        await UniTask.WaitForSeconds(3);
        if (player.IsAlive.Value)
            return;
        SpawnDevice(player, "0");
    }

    protected virtual void PreGameStart(){

    }

    protected virtual void GameStart(){

    }
}
