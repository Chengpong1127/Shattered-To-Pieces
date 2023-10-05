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
    public event Action<BasePlayer> OnPlayerExitGame;
    public StateMachine<GameStates> StateMachine;
    public BaseGameEventHandler[] GameEventHandlers;
    public enum GameStates{
        Initializing,
        CreatingPlayers,
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
        StateMachine.ChangeState(GameStates.Initializing);
        GameEventHandlers.ToList().ForEach(handler => handler.enabled = false);
    }
    public async void RunGame(){
        if (IsServer){
            GameEventHandlers.ToList().ForEach(handler => handler.enabled = true);
            StateMachine.ChangeState(GameStates.CreatingPlayers);
            await CreateAllPlayers();
            StateMachine.ChangeState(GameStates.Gaming);
        }
        else{
            Debug.LogError("GameRunner is not running on server");
        }
    } 
    /// <summary>
    /// Server loads all players.
    /// </summary>
    /// <returns></returns>
    private async UniTask CreateAllPlayers(){
        var playerSpawner = new PlayerSpawner();
        PlayerMap = playerSpawner.SpawnAllPlayers();
        PlayerMap.Values.ToList().ForEach(player => player.OnPlayerDied += () => PlayerDiedHandler(player));
        GameStartSpawnAllPlayer();
        PlayerMap.Keys.ToList().ForEach(playerID => OnPlayerSpawned?.Invoke(playerID));
        await UniTask.WaitUntil(() => PlayerMap.Values.All(player => player.IsAlive.Value));
    }
    protected virtual void GameStartSpawnAllPlayer(){
        PlayerMap.Values.ToList().ForEach(player => SpawnDevice(player, "0"));
    }

    public virtual void SpawnDevice(BasePlayer player, string filename){
        player.ServerLoadDevice(filename);
    }

    protected virtual void PlayerDiedHandler(BasePlayer player){
    }
    protected virtual void PlayerExitGame(BasePlayer player){
        player.OnPlayerDied -= () => PlayerDiedHandler(player);
        PlayerMap.Remove(player.OwnerClientId);
        OnPlayerExitGame?.Invoke(player);
        Debug.Log($"Player with id {player.OwnerClientId} exit the game");
    }
    public void EndGame(){
        StateMachine.ChangeState(GameStates.GameEnd);
    }

}
