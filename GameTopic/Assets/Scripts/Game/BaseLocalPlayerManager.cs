using Unity.Netcode;
using UnityEngine;
using Cysharp.Threading.Tasks;
using UnityEngine.Events;
using System;
using MonsterLove.StateMachine;

public class BaseLocalPlayerManager : NetworkBehaviour
{
    public static BaseLocalPlayerManager RoomInstance { get; private set; }
    public int PlayerCount = 1;
    public BasePlayer Player { get; private set; }
    [SerializeField]
    protected BaseConnectionManager connectionManager;
    public BaseGameRunner GameRunner;
    public event Action OnPlayerExitRoom;
    public StateMachine<PlayerStatus> StateMachine;
    public enum PlayerStatus
    {
        Loading,
        Playing,
        Exiting
    }

    public void Awake()
    {
        Application.targetFrameRate = 30;
        StateMachine = StateMachine<PlayerStatus>.Initialize(this);
        StateMachine.ChangeState(PlayerStatus.Loading);
        if (RoomInstance != null)
        {
            Debug.LogError("There is more than one local player manager in the scene.");
        }
        RoomInstance = this;
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
            StateMachine.ChangeState(PlayerStatus.Playing);
        }
    }
    public virtual void ExitGame(){
        if(IsOwner){
            StateMachine.ChangeState(PlayerStatus.Exiting);
            connectionManager.StopConnection();
            GameEvents.LocalPlayerEvents.OnPlayerRequestExitGame.Invoke();
            OnPlayerExitRoom?.Invoke();
        }
    }
}