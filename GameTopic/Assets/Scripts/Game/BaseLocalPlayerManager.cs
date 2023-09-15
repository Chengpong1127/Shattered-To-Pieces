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
    public bool IsLocalPlayerCompleteSetup { get; private set; } = false;
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
        if(playerID == OwnerClientId){
            Player = Utils.GetLocalPlayerDevice();
            PlayerSpawnSetup();
            IsLocalPlayerCompleteSetup = true;
        }
    }
    /// <summary>
    /// This method will be invoked after the local player is loaded.
    /// </summary>
    protected virtual void PlayerSpawnSetup(){
    }
    public void RequestExitGame(){
        if(IsOwner){
            PreExitGame();
            connectionManager.StopConnection();
            LocalGameManager.Instance.RequestExitRoom();
        }
    }
    protected virtual void PreExitGame(){
    }
}