using Unity.Netcode;
using UnityEngine;
using Cysharp.Threading.Tasks;
using UnityEngine.Events;
using System;

public class BaseLocalPlayerManager : NetworkBehaviour
{
    public int PlayerCount = 1;
    public BasePlayer Player { get; private set; }
    [SerializeField]
    protected BaseConnectionManager connectionManager;
    public BaseGameRunner GameRunner;
    public event Action OnPlayerExitRoom;
    
    public void Awake()
    {
        Application.targetFrameRate = 30;
        if (connectionManager == null)
        {
            Debug.LogError("There is no connection manager in local player manager.");
        }
        GameRunner ??= FindObjectOfType<BaseGameRunner>() ?? throw new Exception("GameRunner is null");

    }
    /// <summary>
    /// Start the player setup. This method need to be invoked after enter a scene.
    /// </summary>
    public void StartPlayerSetup(NetworkType type){
        connectionManager.StartConnection(type);
        connectionManager.OnAllClientConnected += () => {
            if(IsServer){
                SetRunner();
            }
        };
    }
    private void SetRunner(){
        GameRunner.OnPlayerSpawned += PlayerSpawnedHandlerClientRpc;
        GameRunner.OnPlayerExitGame += player => PlayerExitGameHandler_ClientRpc(player.OwnerClientId);
        GameRunner.RunGame();
    }
    [ClientRpc]
    private void PlayerSpawnedHandlerClientRpc(ulong playerID){
        PlayerSpawnedHandler(playerID);
    }
    [ClientRpc]
    private void PlayerExitGameHandler_ClientRpc(ulong playerID){
        if (OwnerClientId == playerID){
            ExitGame();
        }
    }
    private async void PlayerSpawnedHandler(ulong playerID){
        if(playerID == OwnerClientId){
            Player = Utils.GetLocalPlayerDevice();
            await UniTask.WaitUntil(() => Player.IsAlive.Value);
            PlayerSpawnSetup();
        }
    }
    /// <summary>
    /// This method will be invoked after the local player is loaded.
    /// </summary>
    protected virtual void PlayerSpawnSetup(){
    }
    public virtual void ExitGame(){
        if(IsOwner){
            connectionManager.StopConnection();
            GameEvents.LocalPlayerEvents.OnPlayerRequestExitGame.Invoke();
            OnPlayerExitRoom?.Invoke();
        }
    }
}