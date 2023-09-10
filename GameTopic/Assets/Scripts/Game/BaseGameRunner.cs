using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Collections;
using Cysharp.Threading.Tasks;
using Unity.Netcode;

/// <summary>
/// The game runner is responsible for running the game. It will be run on the server.
/// </summary>
public class BaseGameRunner: NetworkBehaviour{
    public INetworkConnector connectionManager;
    protected Dictionary<ulong, IPlayer> PlayerMap;
    public BaseLocalPlayerManager localPlayerManager;
    void Start()
    {
        connectionManager = GetComponent<INetworkConnector>();
        Debug.Assert(connectionManager != null, "connectionManager is null");
        connectionManager.StartConnection();
        connectionManager.OnAllPlayerConnected += async () => {
            if (IsServer){
                GameInitialize();
                await LoadPlayer();
                PreGameStart();
                GameStart();
            }
        };
    }
    protected virtual void GameInitialize(){
        
    }

    private async UniTask LoadPlayer(){
        var playerSpawner = new PlayerSpawner();
        PlayerMap = playerSpawner.SpawnAllPlayers();
        LocalPlayerSetup();
        await UniTask.WaitUntil(() => PlayerMap.Values.All(player => player.IsLoaded));
    }
    private void LocalPlayerSetup(){
        localPlayerManager.LocalPlayerSetup_ClientRpc();
    }

    protected virtual void PreGameStart(){

    }

    protected virtual void GameStart(){

    }
}
