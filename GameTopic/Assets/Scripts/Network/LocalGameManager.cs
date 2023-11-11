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
    
    public enum GameState{
        Init,
        Home,
        Lobby,
        GameRoom,
    }
    public BaseSceneLoader SceneLoader;
    public StateMachine<GameState> StateMachine;
    public LobbyManager LobbyManager;
    private string _startSceneName;

    async void Start()
    {
        Debug.Assert(SceneLoader != null);
        StateMachine = new StateMachine<GameState>(this);
        StateMachine.ChangeState(GameState.Init);
        DontDestroyOnLoad(this);
        _startSceneName = SceneManager.GetActiveScene().name;

        var player = await PlayerSignIn();
        LobbyManager = new LobbyManager(player);
        StateMachine.ChangeState(GameState.Home);
    }

    public async UniTask<Player> PlayerSignIn(){
        await UnityServices.InitializeAsync();
        AuthenticationService.Instance.ClearSessionToken();
        AuthenticationService.Instance.SignedIn += () => {
            Debug.Log("Signed In with ID: " + AuthenticationService.Instance.PlayerId);
        };
        await AuthenticationService.Instance.SignInAnonymouslyAsync();
        var player = new Player(AuthenticationService.Instance.PlayerId, profile: new PlayerProfile("Player"));
        return player;
    }
    #region Lobby

    public async void CreateLobby(string lobbyName){
        StateMachine.ChangeState(GameState.Lobby);
        LobbyManager.OnPlayerJoinOrLeave += () => {
            Debug.Log("Player Join or Leave");
        };
        LobbyManager.OnPlayerReady += player => {
            Debug.Log("Player " + player.Id + " is ready");
        };
        LobbyManager.OnPlayerUnready += player => {
            Debug.Log("Player " + player.Id + " is unready");
        };

        LobbyManager.OnLobbyReady += LobbyReadyHandler;
        await LobbyManager.CreateLobby(lobbyName, ResourceManager.Instance.LoadMapInfo("FightingMap"));
        Debug.Log("Lobby Created with ID: " + LobbyManager.CurrentLobby.Id);
    }
    private void LobbyReadyHandler(LobbyManager.PlayerLobbyReadyInfo playerLobbyReadyInfo){
        LobbyManager.OnLobbyReady -= LobbyReadyHandler;
        NetworkType networkType;
        if (playerLobbyReadyInfo.Identity == LobbyManager.LobbyIdentity.Host){
            LobbyManager.StartGame();
            networkType = NetworkType.Host;
        }else{
            networkType = NetworkType.Client;
        }
        Debug.Log("Lobby Start Game. Host Address: " + playerLobbyReadyInfo.HostAddress + " NetworkType: " + networkType);
        var mapInfo = ResourceManager.Instance.LoadMapInfo(playerLobbyReadyInfo.MapName);
        EnterRoom(mapInfo, networkType, playerLobbyReadyInfo.HostAddress);
    }

    public async UniTask<Lobby[]> GetAllAvailableLobby(){
        return await LobbyManager.GetAllAvailableLobby();
    }

    public async void JoinLobby(Lobby lobby){
        StateMachine.ChangeState(GameState.Lobby);
        LobbyManager.OnPlayerReady += player => {
            Debug.Log("Player " + player.Id + " is ready");
        };
        LobbyManager.OnPlayerReady += player => {
            Debug.Log("Player " + player.Id + " is ready");
        };
        LobbyManager.OnPlayerUnready += player => {
            Debug.Log("Player " + player.Id + " is unready");
        };
        await LobbyManager.JoinLobby(lobby);

        LobbyManager.OnLobbyReady += LobbyReadyHandler;
        Debug.Log("Lobby Joined with ID: " + LobbyManager.CurrentLobby.Id);
    }

    public void PlayerReady(){
        Debug.Assert(StateMachine.State == GameState.Lobby);
        LobbyManager.PlayerReady().Forget();
        Debug.Log("Player Ready");
    }
    public void PlayerUnready(){
        Debug.Assert(StateMachine.State == GameState.Lobby);
        LobbyManager.PlayerUnready().Forget();
        Debug.Log("Player Unready");
    }

    public void PlayerLeave(){
        
    }

    #endregion

    #region GameRoom

    public void EnterAssemblyRoom(){
        var info = ResourceManager.Instance.LoadMapInfo("AssemblyRoom");
        EnterRoom(info, NetworkType.Host);
    }

    public void EnterRoom(MapInfo mapInfo, NetworkType networkType, string ServerAddress = null){
        StateMachine.ChangeState(GameState.GameRoom);
        var operation = SceneManager.LoadSceneAsync(mapInfo.MapSceneName);
        SceneLoader?.LoadScene(operation);
        operation.completed += _ => OnEnterRoom(networkType, mapInfo, ServerAddress);
    }

    private void OnEnterRoom(NetworkType networkType, MapInfo mapInfo, string ServerAddress){
        var playerManager = FindObjectOfType<LocalPlayerManager>();
        playerManager.OnPlayerExitRoom += RequestExitRoom;
        playerManager.StartPlayerSetup(networkType, mapInfo, ServerAddress);
    }



    public void RequestExitRoom(){
        StateMachine.ChangeState(GameState.Home);
        var operation = SceneManager.LoadSceneAsync(_startSceneName);
        SceneLoader?.LoadScene(operation);
    }
    #endregion
}