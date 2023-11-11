using System.Linq;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;

public class StartSceneUIManager : MonoBehaviour
{
    [SerializeField]
    private BaseGamePanel LobbyListPanel;
    [SerializeField]
    private BaseGamePanel CreateLobbyPanel;

    [SerializeField]
    private LobbyUIManager LobbyUIManager;
    [SerializeField]
    private GameObject NonLobbyPanel;
    

    void Awake()
    {
        Debug.Assert(LobbyListPanel != null);
        Debug.Assert(CreateLobbyPanel != null);
        Debug.Assert(LobbyUIManager != null);
        Debug.Assert(NonLobbyPanel != null);

        LobbyListPanel.gameObject.SetActive(true);
        CreateLobbyPanel.gameObject.SetActive(true);
        LobbyUIManager.gameObject.SetActive(false);
        NonLobbyPanel.SetActive(true);
    }
    void Start()
    {
        LocalGameManager.Instance.StateMachine.Changed += GameStateMachineChangedHandler;
    }
    void OnDestroy()
    {
        LocalGameManager.Instance.StateMachine.Changed -= GameStateMachineChangedHandler;
    }
    private void GameStateMachineChangedHandler(LocalGameManager.GameState state){
        switch(state){
            case LocalGameManager.GameState.Home:
                NonLobbyPanel.SetActive(true);
                break;
            case LocalGameManager.GameState.Lobby:
                NonLobbyPanel.SetActive(false);
                PlayerJoinLobbyHandler();
                break;
            
        }
    }


    public async void ShowLobbyList_ButtonAction()
    {
        LobbyListPanel.EnterScene();
        LobbyListController lobbyListController = LobbyListPanel.GetComponentInChildren<LobbyListController>();
        lobbyListController.OnPlayerSelectLobby += PlayerSelectLobbyHandler;
        lobbyListController.StartDisplay();
        var Lobbies = await LocalGameManager.Instance.GetAllAvailableLobby();
        lobbyListController.SetLobbyList(Lobbies.ToList());
    }
    private void PlayerSelectLobbyHandler(Lobby lobby){
        LobbyListPanel.ExitScene();
        LocalGameManager.Instance.JoinLobby(lobby);
    }
    public void ShowCreateLobby_ButtonAction(){
        CreateLobbyPanel.EnterScene();
        CreateLobbyPanel.GetComponentInChildren<CreateLobbyPanelController>().OnCreateLobby += OnCreateLobbyHandler;
    }
    private void PlayerJoinLobbyHandler(){
        LobbyUIManager.gameObject.SetActive(true);
        LobbyUIManager.SetLobbyManager(LocalGameManager.Instance.LobbyManager);
    }

    public void EnterAssemblyRoom_ButtonAction(){
        LocalGameManager.Instance.EnterAssemblyRoom();
    }
    private void OnCreateLobbyHandler(string lobbyName){
        LocalGameManager.Instance.CreateLobby(lobbyName);
        CreateLobbyPanel.ExitScene();
    }
}