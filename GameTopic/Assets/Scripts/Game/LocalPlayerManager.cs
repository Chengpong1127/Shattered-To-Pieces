using Unity.Netcode;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System;
using MonsterLove.StateMachine;

public class LocalPlayerManager : NetworkBehaviour
{
    public static LocalPlayerManager RoomInstance { get; private set; }
    public BasePlayer Player { get; private set; }
    [SerializeField]
    protected BaseConnectionManager connectionManager;
    public GameRunner GameRunner;
    public event Action<GameResult> OnPlayerExitRoom;
    public StateMachine<LocalPlayerStates> StateMachine;
    private GameResult _gameResult;
    private MapInfo _mapInfo;
    public enum LocalPlayerStates
    {
        Initializing,
        Loading,
        Gaming,
        Exiting
    }

    public void Awake()
    {
        StateMachine = StateMachine<LocalPlayerStates>.Initialize(this);
        StateMachine.ChangeState(LocalPlayerStates.Initializing);
        if (RoomInstance != null)
        {
            Debug.LogError("There is more than one local player manager in the scene.");
        }
        RoomInstance = this;
        Debug.Assert(connectionManager != null, "There is no connection manager in local player manager.");
        GameRunner ??= FindObjectOfType<GameRunner>() ?? throw new Exception("GameRunner is null");
    }
    /// <summary>
    /// Start the player setup. This method need to be invoked after enter a scene.
    /// </summary>
    public void StartPlayerSetup(NetworkType type, MapInfo mapInfo, string ServerAddress){
        _mapInfo = mapInfo;
        StateMachine.ChangeState(LocalPlayerStates.Loading);
        connectionManager.StartConnection(type, ServerAddress, mapInfo.MapPlayerCount);
        connectionManager.OnAllClientConnected += AllClientConnectedHandler;
    }
    private void AllClientConnectedHandler(){
        connectionManager.OnAllClientConnected -= AllClientConnectedHandler;
        if(IsServer){
            SetRunner();
        }
    }
    private async void SetRunner(){
        GameRunner.OnGameOver += GameOverHandler_ClientRpc;
        GameRunner.RunGame();
        await UniTask.WaitUntil(() => GameRunner.StateMachine.State == GameRunner.GameStates.Gaming);
        SetGaming_ClientRpc();
    }
    [ClientRpc]
    private void SetGaming_ClientRpc(){
        Player = Utils.GetLocalPlayer();
        StateMachine.ChangeState(LocalPlayerStates.Gaming);
    }
    public virtual void ExitGame(){
        StateMachine.ChangeState(LocalPlayerStates.Exiting);
        connectionManager.StopConnection();
        if (_gameResult == null){
            _gameResult = new GameResult(){
                IsGameAborted = true
            };
        }
        OnPlayerExitRoom?.Invoke(_gameResult);
    }
    protected virtual void GameOverHandler(GameResult result){
        ExitGame();
    }
    [ClientRpc]
    private void GameOverHandler_ClientRpc(GameResult result){
        _gameResult = result;
        _gameResult.SelfPlayerId = OwnerClientId;
        _gameResult.IsGameAborted = false;
        _gameResult.IsRankingGame = _mapInfo.IsRankingMap;
        _gameResult.GameMapName = _mapInfo.MapName;
        GameOverHandler(_gameResult);
    }
}