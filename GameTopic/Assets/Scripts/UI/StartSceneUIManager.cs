using System.Linq;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;

public class StartSceneUIManager : MonoBehaviour
{
    [SerializeField]
    private GameWidget LobbyListPanel;
    [SerializeField]
    private GameWidget CreateLobbyPanel;

    [SerializeField]
    private LobbyUIManager LobbyUIManager;
    [SerializeField]
    private GameObject HomePanel;
    [SerializeField]
    private NotificationWindowController NotificationWindowController;
    

    void Awake()
    {
        Debug.Assert(LobbyListPanel != null);
        Debug.Assert(CreateLobbyPanel != null);
        Debug.Assert(LobbyUIManager != null);
        Debug.Assert(HomePanel != null);
        Debug.Assert(NotificationWindowController != null);
        LobbyUIManager.OnExitLobby += OnExitLobbyHandler;
        HomePanel.SetActive(true);
    }
    void Start()
    {
        LocalGameManager.Instance.StateMachine.Changed += GameStateMachineChangedHandler;
        EnterHome();
    }
    void OnDestroy()
    {
        if (LocalGameManager.Instance.StateMachine != null)
            LocalGameManager.Instance.StateMachine.Changed -= GameStateMachineChangedHandler;
        LobbyUIManager.OnExitLobby -= OnExitLobbyHandler;
    }
    private void GameStateMachineChangedHandler(LocalGameManager.GameState state){
        switch(state){
            case LocalGameManager.GameState.Home:
                EnterHome();
                break;
            case LocalGameManager.GameState.Lobby:
                EnterLobby();
                break;
            
        }
    }


    public async void ShowLobbyList_ButtonAction()
    {
        LobbyListPanel.Show();
        LobbyListController lobbyListController = LobbyListPanel.GetComponentInChildren<LobbyListController>();
        lobbyListController.OnPlayerSelectLobby += PlayerSelectLobbyHandler;
        lobbyListController.StartDisplay();
        var Lobbies = await LocalGameManager.Instance.GetAllAvailableLobby();
        lobbyListController.SetLobbyList(Lobbies.ToList());
    }
    private async void PlayerSelectLobbyHandler(Lobby lobby){
        LobbyListPanel.GetComponentInChildren<LobbyListController>().OnPlayerSelectLobby -= PlayerSelectLobbyHandler;
        try{
            await LocalGameManager.Instance.JoinLobby(lobby);
            LobbyListPanel.Close();
        }catch(LobbyServiceException){
            await NotificationWindowController.ShowNotification("Join Lobby Failed", "Please try again later");
            LobbyListPanel.Close();
        }
    }
    public void ShowCreateLobby_ButtonAction(){
        CreateLobbyPanel.Show();
        CreateLobbyPanel.GetComponentInChildren<CreateLobbyPanelController>().OnCreateLobby += OnCreateLobbyHandler;
    }
    private void EnterHome(){
        HomePanel.SetActive(true);
        LobbyUIManager.gameObject.SetActive(false);
    }
    private void EnterLobby(){
        HomePanel.SetActive(false);
        LobbyUIManager.gameObject.SetActive(true);
        LobbyUIManager.SetLobbyManager(LocalGameManager.Instance.LobbyManager);
    }

    public void EnterAssemblyRoom_ButtonAction(){
        LocalGameManager.Instance.EnterAssemblyRoom();
    }
    private async void OnCreateLobbyHandler(string lobbyName){
        CreateLobbyPanel.GetComponentInChildren<CreateLobbyPanelController>().OnCreateLobby -= OnCreateLobbyHandler;
        if (lobbyName == "") lobbyName = "Room";
        try{
            await LocalGameManager.Instance.CreateLobby(lobbyName);
            CreateLobbyPanel.Close();
        }catch(LobbyServiceException e){
            Debug.Log(e.Message);
            await NotificationWindowController.ShowNotification("Create Lobby Failed", "Please try again later");
            CreateLobbyPanel.Close();
        }
        
    }
    private void OnExitLobbyHandler(){
        LobbyUIManager.ExitLobbyMode();
        LocalGameManager.Instance.PlayerExitLobby();
    }
}