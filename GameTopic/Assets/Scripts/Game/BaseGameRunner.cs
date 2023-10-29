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

    public MapManager MapManager;
    public event Action<GameResult> OnGameOver;
    public StateMachine<GameStates> StateMachine;
    public BaseGameEventHandler[] GameEventHandlers { get; private set; }
    public static BaseGameRunner ServerGameRunnerInstance { get; private set; }
    public enum GameStates{
        Initializing,
        CreatingPlayers,
        Gaming,
        GameOver
    }
    protected Dictionary<ulong, BasePlayer> PlayerMap;
    /// <summary>
    /// This event will be invoked when a player is spawned. The parameter is the player's network object id.
    /// </summary>
    public event Action<ulong> OnPlayerSpawned;
    void Awake()
    {
        GameEventHandlers = GetComponentsInChildren<BaseGameEventHandler>();
        StateMachine = new StateMachine<GameStates>(this);
        StateMachine.ChangeState(GameStates.Initializing);
        GameEventHandlers.ToList().ForEach(handler => handler.enabled = false);
    }
    public async void RunGame(){
        if (IsServer){
            if (ServerGameRunnerInstance != null){
                Debug.LogError("There is more than one game runner in the scene.");
            }
            ServerGameRunnerInstance = this;
            EnableGameEventHandler();
            EnableGameEventHandler_ClientRpc();
            StateMachine.ChangeState(GameStates.CreatingPlayers);
            await CreateAllPlayers();
            StateMachine.ChangeState(GameStates.Gaming);
        }
        else{
            Debug.LogError("GameRunner is not running on server");
        }
    } 
    [ClientRpc]
    private void EnableGameEventHandler_ClientRpc(){
        EnableGameEventHandler();
    }
    private void EnableGameEventHandler(){
        GameEventHandlers.ToList().ForEach(handler => {
            switch(handler.HandlerRunningMode){
                case BaseGameEventHandler.RunningMode.OnlyServer:
                    handler.enabled = IsServer;
                    break;
                case BaseGameEventHandler.RunningMode.OnlyClient:
                    handler.enabled = IsClient;
                    break;
                case BaseGameEventHandler.RunningMode.Both:
                    handler.enabled = true;
                    break;
            }
        });
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
        PlayerMap.Values.ToList().ForEach(player => {
            SpawnDevice(player, "0");
        });
    }

    public virtual void SpawnDevice(BasePlayer player, string filename){
        player.ServerLoadDevice(filename, Vector3.zero);
    }

    protected virtual void PlayerDiedHandler(BasePlayer player){
    }
    public void GameOver(GameResult result){
        StateMachine.ChangeState(GameStates.GameOver);
        OnGameOver?.Invoke(result);
        ServerGameRunnerInstance = null;
    }

}
