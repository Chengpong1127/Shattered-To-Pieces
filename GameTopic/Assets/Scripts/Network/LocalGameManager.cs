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
        AssemblyRoom,
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
    public void EnterAssemblyRoom(){
        var operation = SceneManager.LoadSceneAsync("AssemblyRoom");
        operation.completed += (AsyncOperation op) => {
            GameStateMachine.ChangeState(GameState.AssemblyRoom);
        };
    }


    private void AssemblyRoom_Enter(){
        Debug.Log("Enter Assembly Room");
        EnterRoom();

    }

    private void EnterRoom(){
        var connectionManager = FindFirstObjectByType<GlobalConnectionManager>();

        connectionManager.StartConnection();
    }
    
}