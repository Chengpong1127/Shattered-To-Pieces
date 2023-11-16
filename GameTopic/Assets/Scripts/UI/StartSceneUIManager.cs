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
    

    void Awake()
    {
        Debug.Assert(LobbyListPanel != null);
        Debug.Assert(CreateLobbyPanel != null);
        Debug.Assert(LobbyUIManager != null);
        Debug.Assert(HomePanel != null);

        LobbyListPanel.gameObject.SetActive(true);
        CreateLobbyPanel.gameObject.SetActive(true);
        LobbyUIManager.OnExitLobby += OnExitLobbyHandler;
        HomePanel.SetActive(true);
    }
    void Start()
    {
        LocalGameManager.Instance.StateMachine.Changed += GameStateMachineChangedHandler;
    }
    void OnDestroy()
    {
        if (LocalGameManager.Instance.StateMachine != null)
            LocalGameManager.Instance.StateMachine.Changed -= GameStateMachineChangedHandler;
    }
    private void GameStateMachineChangedHandler(LocalGameManager.GameState state){
        switch(state){
            case LocalGameManager.GameState.Home:
                LobbyUIManager.gameObject.SetActive(false);
                HomePanel.SetActive(true);
                break;
            case LocalGameManager.GameState.Lobby:
                HomePanel.SetActive(false);
                PlayerEnterLobbyHandler();
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
        LobbyListPanel.Close();
        await LocalGameManager.Instance.JoinLobby(lobby);
    }
    public void ShowCreateLobby_ButtonAction(){
        CreateLobbyPanel.Show();
        CreateLobbyPanel.GetComponentInChildren<CreateLobbyPanelController>().OnCreateLobby += OnCreateLobbyHandler;
    }
    private void PlayerEnterLobbyHandler(){
        LobbyUIManager.gameObject.SetActive(true);
        LobbyUIManager.SetLobbyManager(LocalGameManager.Instance.LobbyManager);
    }

    public void EnterAssemblyRoom_ButtonAction(){
        LocalGameManager.Instance.EnterAssemblyRoom();
    }
    private void OnCreateLobbyHandler(string lobbyName){
        if (lobbyName == "") lobbyName = "Room";
        LocalGameManager.Instance.CreateLobby(lobbyName);
        CreateLobbyPanel.Close();
    }
    private void OnExitLobbyHandler(){
        LobbyUIManager.ExitLobbyMode();
        LocalGameManager.Instance.PlayerExitLobby();
    }
}