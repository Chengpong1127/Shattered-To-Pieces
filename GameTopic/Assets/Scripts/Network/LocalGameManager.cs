using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies.Models;
using System.Threading.Tasks;
using UnityEngine.SceneManagement;
using MonsterLove.StateMachine;
using System.Linq;
using Cysharp.Threading.Tasks;


public class LocalGameManager: SingletonMonoBehavior<LocalGameManager>{
    
    private enum GameState{
        Init,
        Home,
        Lobby,
        GameRoom,
    }
    private StateMachine<GameState> GameStateMachine;
    private LobbyManager LobbyManager;

    async void Start()
    {
        GameStateMachine = new StateMachine<GameState>(this);
        GameStateMachine.ChangeState(GameState.Init);
        DontDestroyOnLoad(this);
        var player = await PlayerSignIn();
        LobbyManager = new LobbyManager(player);
        GameStateMachine.ChangeState(GameState.Home);
    }

    public async UniTask<Player> PlayerSignIn(){
        await UnityServices.InitializeAsync();
        AuthenticationService.Instance.SignedIn += () => {
            Debug.Log("Signed In with ID: " + AuthenticationService.Instance.PlayerId);
        };
        await AuthenticationService.Instance.SignInAnonymouslyAsync();
        var player = new Player(AuthenticationService.Instance.PlayerId);
        return player;
    }
    #region Lobby

    public async void CreateLobby(string lobbyName){
        GameStateMachine.ChangeState(GameState.Lobby);
        LobbyManager.OnPlayerReady += player => {
            Debug.Log("Player " + player.Id + " is ready");
        };
        LobbyManager.OnPlayerUnready += player => {
            Debug.Log("Player " + player.Id + " is unready");
        };

        LobbyManager.OnLobbyReady += LobbyReadyHandler;
        await LobbyManager.CreateLobby(lobbyName, 4);
    }
    private void LobbyReadyHandler(){
        EnterRoom(LobbyManager.GetLobbyMap(), NetworkType.Host);
    }

    public async UniTask<Lobby[]> GetAllAvailableLobby(){
        return await LobbyManager.GetAllAvailableLobby();
    }

    public async void JoinLobby(Lobby lobby){
        GameStateMachine.ChangeState(GameState.Lobby);
        await LobbyManager.JoinLobby(lobby);

        LobbyManager.OnLobbyReady += LobbyReadyHandler;
    }

    public void PlayerReady(){
        Debug.Assert(GameStateMachine.State == GameState.Lobby);
        LobbyManager.PlayerReady();
    }
    public void PlayerUnready(){
        Debug.Assert(GameStateMachine.State == GameState.Lobby);
        LobbyManager.PlayerUnready();
    }

    public void PlayerLeave(){
        
    }

    private string GetServerAddress(){
        return LobbyManager.GetLobbyHostIP();
    }

    #endregion

    #region GameRoom
    public void EnterRoom(string roomName, NetworkType networkType, string ServerAddress = null){
        GameStateMachine.ChangeState(GameState.GameRoom);
        var operation = SceneManager.LoadSceneAsync(roomName);
        operation.completed += _ => OnEnterRoom(networkType, ServerAddress);
    }

    private void OnEnterRoom(NetworkType networkType, string ServerAddress = null){
        var playerManager = FindObjectOfType<BaseLocalPlayerManager>();
        playerManager.OnPlayerExitRoom += RequestExitRoom;
        playerManager.StartPlayerSetup(networkType, ServerAddress);
    }



    public void RequestExitRoom(){
        GameStateMachine.ChangeState(GameState.Home);
        SceneManager.LoadSceneAsync("StartScene");
    }
    #endregion
}