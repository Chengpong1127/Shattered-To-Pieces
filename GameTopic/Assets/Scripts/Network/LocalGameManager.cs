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
    private Player SelfPlayer;

    protected override void Awake() {
        base.Awake();
        GameStateMachine = new StateMachine<GameState>(this);
        GameStateMachine.ChangeState(GameState.Init);
        DontDestroyOnLoad(this);
    }
    public async Task PlayerSignIn(){
        await UnityServices.InitializeAsync();
        AuthenticationService.Instance.SignedIn += () => {
            Debug.Log("Signed In with ID: " + AuthenticationService.Instance.PlayerId);
        };
        await AuthenticationService.Instance.SignInAnonymouslyAsync();
        SelfPlayer = new Player(AuthenticationService.Instance.PlayerId);
        GameStateMachine.ChangeState(GameState.Home);
    }
    #region Lobby

    public async void CreateLobby(string lobbyName){
        GameStateMachine.ChangeState(GameState.Lobby);
        await LobbyManager.Instance.CreateLobby(lobbyName, 4, SelfPlayer);
    }

    public async UniTask<Lobby[]> GetAllAvailableLobby(){
        return await LobbyManager.Instance.GetAllAvailableLobby();
    }

    public async void JoinLobby(Lobby lobby){
        GameStateMachine.ChangeState(GameState.Lobby);
        await LobbyManager.Instance.JoinLobby(lobby, SelfPlayer);
    }

    #endregion

    #region GameRoom
    public void EnterRoom(string roomName, NetworkType networkType){
        GameStateMachine.ChangeState(GameState.GameRoom);
        var operation = SceneManager.LoadSceneAsync(roomName);
        operation.completed += _ => OnEnterRoom(networkType);
    }

    private void OnEnterRoom(NetworkType networkType){
        var playerManager = FindObjectOfType<BaseLocalPlayerManager>();
        playerManager.OnPlayerExitRoom += RequestExitRoom;
        playerManager.StartPlayerSetup(networkType);
    }



    public void RequestExitRoom(){
        GameStateMachine.ChangeState(GameState.Home);
        SceneManager.LoadSceneAsync("StartScene");
    }
    #endregion
}