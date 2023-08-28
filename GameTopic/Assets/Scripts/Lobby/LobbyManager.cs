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

public class LobbyManager : MonoBehaviour
{
    private Lobby m_CurrentLobby;
    private Player m_CurrentPlayer;
    private Allocation allocation;
    void Start()
    {
        SignIn();
    }
    public async void SignIn(){
        await UnityServices.InitializeAsync();
        AuthenticationService.Instance.SignedIn += () => {
            Debug.Log("Signed In " + AuthenticationService.Instance.PlayerId);
        };
        await AuthenticationService.Instance.SignInAnonymouslyAsync();
        m_CurrentPlayer = new Player(AuthenticationService.Instance.PlayerId);
    }
    public async void CreateLobby(){
        string lobbyName = "My Lobby";
        int maxPlayers = 4;
        CreateLobbyOptions createLobbyOptions = new CreateLobbyOptions { Player = m_CurrentPlayer };
        m_CurrentLobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, maxPlayers, createLobbyOptions);
        BindLobbyEvents(m_CurrentLobby.Id);
        Debug.Log("Lobby Created " + m_CurrentLobby.Name + " " + m_CurrentLobby.LobbyCode);
    }
    public async void ListAllLobbies(){
        var responce = await LobbyService.Instance.QueryLobbiesAsync();
        Debug.Log("Lobby Count " + responce.Results.Count);
        foreach (var lobby in responce.Results)
        {
            Debug.Log("Lobby Name: " + lobby.Name + " Lobby Code: " + lobby.LobbyCode);
        }
    }

    public async void JoinLobby(){
        if (m_CurrentLobby != null)
        {
            Debug.Log("Already Joined Lobby");
            return;
        }
        QuickJoinLobbyOptions quickJoinLobbyOptions = new QuickJoinLobbyOptions { Player = m_CurrentPlayer };
        m_CurrentLobby = await LobbyService.Instance.QuickJoinLobbyAsync(quickJoinLobbyOptions);
        Debug.Log("Joined Lobby " + m_CurrentLobby.Name);
        BindLobbyEvents(m_CurrentLobby.Id);
    }
    public void ListLobbyPlayers(){
        if (m_CurrentLobby == null)
        {
            Debug.Log("No Lobby Joined");
            return;
        }
        Debug.Log(m_CurrentLobby.Players.Count);
        foreach (var player in m_CurrentLobby.Players)
        {
            Debug.Log("Player Id: " + player.Id);
        }
    }

    public async void StartGame(){
        Debug.Log("Starting Game with players:" + m_CurrentLobby.Players.Count);
        string relayCode = await CreateRelay(m_CurrentLobby.Players.Count);
        
        await AddRelayCodeAsync(relayCode);

        await Task.Delay(4000);
        BeginGame();
    }

    private async Task<string> CreateRelay(int maxConnections){
        Debug.Log("Creating Relay");
        allocation = await RelayService.Instance.CreateAllocationAsync(maxConnections);
        var joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
        RelayServerData data = new RelayServerData(allocation, "dtls");
        NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(data);
        NetworkManager.Singleton.StartHost();

        
        return joinCode;
    }
    private async void JoinRelay(string joinCode){
        if (NetworkManager.Singleton.IsHost) return;
        Debug.Log("Joining Relay");
        JoinAllocation allocation = await RelayService.Instance.JoinAllocationAsync(joinCode);
        RelayServerData data = new RelayServerData(allocation, "dtls");
        NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(data);
        NetworkManager.Singleton.StartClient();
        await UpdatePlayerDataAsync("ConnectionOK", "true");
    }
    private void BeginGame(){
        Debug.Log("Starting Game");
        NetworkManager.Singleton.SceneManager.LoadScene("NetworkTestScene", LoadSceneMode.Single);
    }

    private void BindLobbyEvents(string lobbyID){
        LobbyEventCallbacks LobbyEventCallbacks = new LobbyEventCallbacks();
        LobbyEventCallbacks.LobbyChanged += (changes) => {
            Debug.Log("Lobby Changed");
        };
        LobbyEventCallbacks.PlayerJoined += (players) => {
            Debug.Log("Player Joined");
        };
        LobbyEventCallbacks.PlayerLeft += (players) => {
            Debug.Log("Player Left");
        };
        LobbyEventCallbacks.DataChanged += (data) => {
            Debug.Log("Data Changed");
        };
        LobbyEventCallbacks.DataRemoved += (data) => {
            Debug.Log("Data Removed");
        };
        LobbyEventCallbacks.DataAdded += (data) => {
            Debug.Log("Data Added");
            if (data.TryGetValue("relayCode", out var relayCode))
            {
                JoinRelay(relayCode.Value.Value);
            }
        };
        LobbyEventCallbacks.PlayerDataChanged += (data) => {
            Debug.Log("Player Data Changed");
        };
        LobbyEventCallbacks.PlayerDataRemoved += (data) => {
            Debug.Log("Player Data Removed");
        };
        LobbyEventCallbacks.PlayerDataAdded += (data) => {
            Debug.Log("Player Data Added");
        };
        LobbyEventCallbacks.LobbyDeleted += () => {
            Debug.Log("Lobby Deleted");
        };
        LobbyEventCallbacks.KickedFromLobby += () => {
            Debug.Log("Kicked From Lobby");
        };
        LobbyEventCallbacks.LobbyEventConnectionStateChanged += (state) => {
            Debug.Log("Lobby Event Connection State Changed " + state);
        };
        LobbyService.Instance.SubscribeToLobbyEventsAsync(lobbyID, LobbyEventCallbacks);
    }

    public async Task AddRelayCodeAsync(string relayCode)
    {
        await UpdateLobbyDataAsync("relayCode", relayCode);
    }

    public async Task UpdateLobbyDataAsync(string key, string value, bool isPrivate = false)
    {
        if (m_CurrentLobby == null)
            return;

        Dictionary<string, DataObject> dataCurr = m_CurrentLobby.Data ?? new Dictionary<string, DataObject>();
        dataCurr[key] = new DataObject(isPrivate ? DataObject.VisibilityOptions.Private : DataObject.VisibilityOptions.Public, value);

        UpdateLobbyOptions updateOptions = new UpdateLobbyOptions { Data = dataCurr };
        m_CurrentLobby = await LobbyService.Instance.UpdateLobbyAsync(m_CurrentLobby.Id, updateOptions);
    }

    public async Task UpdatePlayerDataAsync(string key, string value, bool isPrivate = false)
    {
        if (m_CurrentLobby == null)
            return;

        Dictionary<string, PlayerDataObject> dataCurr = m_CurrentPlayer.Data ?? new Dictionary<string, PlayerDataObject>();
        dataCurr[key] = new PlayerDataObject(isPrivate ? PlayerDataObject.VisibilityOptions.Private : PlayerDataObject.VisibilityOptions.Public, value);

        UpdatePlayerOptions updateOptions = new UpdatePlayerOptions { Data = dataCurr };
        await LobbyService.Instance.UpdatePlayerAsync(m_CurrentLobby.Id, m_CurrentPlayer.Id, updateOptions);
    }

}
