using Unity.Netcode;
using UnityEngine;
using Cysharp.Threading.Tasks;
using UnityEngine.Events;
using System;

public class BaseLocalPlayerManager : NetworkBehaviour
{
    public PlayerDevice Player { get; private set; }
    public bool IsLocalPlayerCompleteSetup { get; private set; } = false;
    public UnityEvent OnLocalPlayerLoaded;
    public string InitLoadDeviceName = "0";
    public event Action OnPlayerRequestExitGame;
    protected INetworkConnector connectionManager;
    public BaseGameRunner GameRunner;
    public void Awake()
    {
        connectionManager = gameObject.AddComponent<GlobalConnectionManager>();
        GameRunner = GetComponent<BaseGameRunner>();
    }
    public void StartPlayerSetup(int playerCount){
        connectionManager.StartConnection(playerCount);
        connectionManager.OnAllDeviceConnected += () => {
            if(IsServer){
                SetRunner();
            }
        };
    }
    private void SetRunner(){
        GameRunner.OnAllPlayerSpawned += LocalPlayerSetup_ClientRpc;
        GameRunner.RunGame();
    }
    [ClientRpc]
    public void LocalPlayerSetup_ClientRpc()
    {
        WaitPlayerLoaded();
    }
    private async void WaitPlayerLoaded(){
        if(IsOwner){
            Player = Utils.GetLocalPlayerDevice();
            Debug.Assert(Player != null, "Player is null");
            Player.LoadLocalDevice(InitLoadDeviceName);
            await UniTask.WaitUntil(() => Player.IsLoaded);
            PlayerSetup();
            IsLocalPlayerCompleteSetup = true;
            OnLocalPlayerLoaded?.Invoke();
        }

    }
    /// <summary>
    /// This method will be invoked after the local player is loaded.
    /// </summary>
    protected virtual void PlayerSetup(){
    }
    public void RequestExitGame(){
        if(IsOwner){
            PreExitGame();
            OnPlayerRequestExitGame?.Invoke();
            connectionManager.StopConnection();
            LocalGameManager.Instance.RequestExitRoom();
        }
    }
    protected virtual void PreExitGame(){
    }
}