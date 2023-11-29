using System.Linq;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;

public class StartSceneUIManager : MonoBehaviour
{
    [SerializeField]
    private LobbyListController LobbyListController;
    [SerializeField]
    private CreateLobbyPanelController CreateLobbyPanelController;

    [SerializeField]
    private LobbyUIManager LobbyUIManager;
    [SerializeField]
    private GameObject HomePanel;
    [SerializeField]
    private NotificationWindowController NotificationWindowController;
    [SerializeField]
    private LoadingUIController LoadingUIController;
    [SerializeField]
    private PlayerProfileController PlayerProfileController;

    [SerializeField]
    private Text _nameText;
    

    void Awake()
    {
        Debug.Assert(LobbyListController != null);
        Debug.Assert(CreateLobbyPanelController != null);
        Debug.Assert(LobbyUIManager != null);
        Debug.Assert(HomePanel != null);
        Debug.Assert(NotificationWindowController != null);
        Debug.Assert(LoadingUIController != null);
        Debug.Assert(PlayerProfileController != null);
        Debug.Assert(_nameText != null);
        LobbyUIManager.OnExitLobby += OnExitLobbyHandler;
        CreateLobbyPanelController.OnCreateLobby += OnCreateLobbyHandler;
        LobbyListController.OnPlayerSelectLobby += PlayerSelectLobbyHandler;
        GameEvents.OnPlayerProfileUpdated += PlayerProfileUpdatedHandler;
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
        CreateLobbyPanelController.OnCreateLobby -= OnCreateLobbyHandler;
        LobbyListController.OnPlayerSelectLobby -= PlayerSelectLobbyHandler;
        LobbyUIManager.OnExitLobby -= OnExitLobbyHandler;
        GameEvents.OnPlayerProfileUpdated -= PlayerProfileUpdatedHandler;
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
        LobbyListController.Show();
        var Lobbies = await LocalGameManager.Instance.GetAllAvailableLobby();
        LobbyListController.SetLobbyList(Lobbies.ToList());
    }
    private async void PlayerSelectLobbyHandler(Lobby lobby){
        try{
            LoadingUIController.ShowLoading();
            await LocalGameManager.Instance.JoinLobby(lobby);
            LoadingUIController.FinishLoading();
            LobbyListController.Close();
        }catch(LobbyServiceException){
            await NotificationWindowController.ShowNotification("Join Lobby Failed", "Please try again later");
            LoadingUIController.FinishLoading();
            LobbyListController.Close();
        }
    }
    public void ShowCreateLobby_ButtonAction(){
        CreateLobbyPanelController.Show();
    }
    private void EnterHome(){
        HomePanel.SetActive(true);
        LobbyUIManager.gameObject.SetActive(false);

        var profile = ResourceManager.Instance.LoadLocalPlayerProfile();
        _nameText.text = profile.Name;
    }
    private void EnterLobby(){
        HomePanel.SetActive(false);
        LobbyUIManager.gameObject.SetActive(true);
        LobbyUIManager.SetLobbyManager(LocalGameManager.Instance.LobbyManager);
    }

    public void EnterAssemblyRoom_ButtonAction(){
        LocalGameManager.Instance.EnterAssemblyRoom();
    }
    private async void OnCreateLobbyHandler(string lobbyName, MapInfo mapInfo){
        if (lobbyName == "") lobbyName = "Room";
        try{
            LoadingUIController.ShowLoading();
            await LocalGameManager.Instance.CreateLobby(lobbyName, mapInfo);
            CreateLobbyPanelController.Close();
            LoadingUIController.FinishLoading();
        }catch(LobbyServiceException e){
            Debug.Log(e.Message);
            await NotificationWindowController.ShowNotification("Create Lobby Failed", "Please try again later");
            CreateLobbyPanelController.Close();
            LoadingUIController.FinishLoading();
        }
        
    }
    private void OnExitLobbyHandler(){
        LobbyUIManager.ExitLobbyMode();
        LocalGameManager.Instance.PlayerExitLobby();
    }
    private void PlayerProfileUpdatedHandler(){
        var profile = ResourceManager.Instance.LoadLocalPlayerProfile();
        _nameText.text = profile.Name;
    }

    public void QuitGame_ButtonAction(){
        Application.Quit();
    }

}