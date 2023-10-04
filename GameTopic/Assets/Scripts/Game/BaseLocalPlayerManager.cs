using Unity.Netcode;
using UnityEngine;
using Cysharp.Threading.Tasks;
using UnityEngine.Events;
using System;

public class BaseLocalPlayerManager : NetworkBehaviour
{
    public int PlayerCount = 1;
    public bool RunAtStart = false;
    public BasePlayer Player { get; private set; }
    protected INetworkConnector connectionManager;
    public BaseGameRunner GameRunner;
    public event Action OnPlayerExitRoom;
    
    public void Awake()
    {
        Application.targetFrameRate = 30;
        connectionManager = gameObject.AddComponent<GlobalConnectionManager>();
        GameRunner ??= FindObjectOfType<BaseGameRunner>() ?? throw new Exception("GameRunner is null");

    }
    public void Start(){
        if(RunAtStart){
            StartPlayerSetup();
        }
    }
    /// <summary>
    /// Start the player setup. This method need to be invoked after enter a scene.
    /// </summary>
    public void StartPlayerSetup(){
        connectionManager.StartConnection(PlayerCount);
        connectionManager.OnAllDeviceConnected += () => {
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