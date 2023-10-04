using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using System.Net;
using Unity.Services.Lobbies.Models;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Services.Relay.Models;
using Unity.Services.Relay;
using Unity.Networking.Transport.Relay;
using System.Threading.Tasks;
using UnityEngine.SceneManagement;
using MonsterLove.StateMachine;
using System.Linq;


public class LocalGameManager: SingletonMonoBehavior<LocalGameManager>{
    
    private enum GameState{
        Init,
        Home,
        GameRoom,
    }
    private StateMachine<GameState> GameStateMachine;
    private Player SelfPlayer;

    private void Awake() {
        
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
    private void Home_Enter(){
        Debug.Log("Enter Home");
        SceneManager.LoadSceneAsync("StartScene");
    }

    public void EnterRoom(string roomName){
        GameStateMachine.ChangeState(GameState.GameRoom);
        var operation = SceneManager.LoadSceneAsync(roomName);
        operation.completed += _ => OnEnterRoom();
    }

    private void OnEnterRoom(){
        var playerManager = FindObjectOfType<BaseLocalPlayerManager>();
        playerManager.OnPlayerExitRoom += RequestExitRoom;
        if (!playerManager.RunAtStart)
            playerManager.StartPlayerSetup();
    }



    public void RequestExitRoom(){
        GameStateMachine.ChangeState(GameState.Home);
    }
    
    public void DeleteReduntantNetworkManagers(){
        var networkManagers = FindObjectsOfType<NetworkManager>();
        foreach(var manager in networkManagers){
            if(manager.gameObject != gameObject){
                Destroy(manager.gameObject);
            }
        }
    }
}