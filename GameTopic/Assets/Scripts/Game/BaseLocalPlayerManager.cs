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
    public string InitLoadDeviceName = "0";
    protected INetworkConnector connectionManager;
    public BaseGameRunner GameRunner;
    
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
        GameRunner.RunGame();
    }
    [ClientRpc]
    private void PlayerSpawnedHandlerClientRpc(ulong playerID){
        PlayerSpawnedHandler(playerID);
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
        }
    }
}