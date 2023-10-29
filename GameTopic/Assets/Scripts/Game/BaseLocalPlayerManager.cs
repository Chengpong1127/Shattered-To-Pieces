using Unity.Netcode;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System;
using MonsterLove.StateMachine;

public class BaseLocalPlayerManager : NetworkBehaviour
{
    public static BaseLocalPlayerManager RoomInstance { get; private set; }
    public BasePlayer Player { get; private set; }
    [SerializeField]
    protected BaseConnectionManager connectionManager;
    public BaseGameRunner GameRunner;
    public event Action OnPlayerExitRoom;
    public StateMachine<LocalPlayerStates> StateMachine;
    public enum LocalPlayerStates
    {
        Initializing,
        Loading,
        Playing,
        Exiting
    }

    public void Awake()
    {
        Application.targetFrameRate = 30;
        StateMachine = StateMachine<LocalPlayerStates>.Initialize(this);
        StateMachine.ChangeState(LocalPlayerStates.Initializing);
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
    public void StartPlayerSetup(NetworkType type, string ServerAddress = null){
        StateMachine.ChangeState(LocalPlayerStates.Loading);
        connectionManager.StartConnection(type, ServerAddress);
        connectionManager.OnAllClientConnected += () => {
            if(IsServer){
                SetRunner();
            }
        };
    }
    private void SetRunner(){
        GameRunner.OnPlayerSpawned += PlayerSpawnedHandlerClientRpc;
        GameRunner.OnGameOver += GameOverHandler_ClientRpc;
        GameRunner.RunGame();
    }
    [ClientRpc]
    private void PlayerSpawnedHandlerClientRpc(ulong playerID){
        PlayerSpawnedHandler(playerID);
    }
    private async void PlayerSpawnedHandler(ulong playerID){
        if(playerID == OwnerClientId){
            Player = Utils.GetLocalPlayer();
            await UniTask.WaitUntil(() => Player.IsAlive.Value);
            StateMachine.ChangeState(LocalPlayerStates.Playing);
        }
    }
    public virtual void ExitGame(){
        StateMachine.ChangeState(LocalPlayerStates.Exiting);
        connectionManager.StopConnection();
        OnPlayerExitRoom?.Invoke();
    }
    protected virtual void GameOverHandler(GameResult result){
        ExitGame();
    }
    [ClientRpc]
    private void GameOverHandler_ClientRpc(GameResult result){
        GameOverHandler(result);
    }
}