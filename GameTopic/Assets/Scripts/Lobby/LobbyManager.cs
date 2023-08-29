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
    public Lobby CurrentLobby;
    public Player SelfPlayer;
    public Allocation Allocation;
    void Awake()
    {
    }
    public async Task SignIn(){
        await UnityServices.InitializeAsync();
        AuthenticationService.Instance.SignedIn += () => {
            Debug.Log("Signed In with ID: " + AuthenticationService.Instance.PlayerId);
        };
        await AuthenticationService.Instance.SignInAnonymouslyAsync();
        SelfPlayer = new Player(AuthenticationService.Instance.PlayerId);
    }
    public async Task CreateLobby(string lobbyName, int maxPlayers){
        CreateLobbyOptions createLobbyOptions = new CreateLobbyOptions { Player = SelfPlayer };
        CurrentLobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, maxPlayers, createLobbyOptions);
        BindLobbyEvents(CurrentLobby.Id);
        Debug.Log("Lobby Created " + CurrentLobby.Name + " " + CurrentLobby.LobbyCode);
    }
    public async void ListAllLobbies(){
        var responce = await LobbyService.Instance.QueryLobbiesAsync();
        Debug.Log("Lobby Count " + responce.Results.Count);
        foreach (var lobby in responce.Results)
        {
            Debug.Log("Lobby Name: " + lobby.Name + " Lobby Code: " + lobby.LobbyCode);
        }
    }
    public async Task<Lobby> GetTheLastestLobby(){
        QueryLobbiesOptions queryLobbiesOptions = new QueryLobbiesOptions {
            Order = new List<QueryOrder> {
                new QueryOrder (field: QueryOrder.FieldOptions.Created, asc: true)
            }
        };
        try{
            var responce = await LobbyService.Instance.QueryLobbiesAsync(queryLobbiesOptions);
            if (responce.Results.Count == 0)
            {
                return null;
            }
            return responce.Results[0];
        }catch (LobbyServiceException e){
            Debug.Log(e.Message);
            return null;
        }
        
        
    }

    public async Task JoinLobby(Lobby lobby){
        if (CurrentLobby != null)
        {
            Debug.Log("Already Joined Lobby");
            return;
        }
        JoinLobbyByIdOptions quickJoinLobbyOptions = new JoinLobbyByIdOptions { Player = SelfPlayer };
        CurrentLobby = await LobbyService.Instance.JoinLobbyByIdAsync(lobby.Id, quickJoinLobbyOptions);
        BindLobbyEvents(CurrentLobby.Id);
        Debug.Log("Joined new lobby: " + CurrentLobby.Name);
    }
    public string GetLobbyRelayCode(){
        if (CurrentLobby == null)
        {
            Debug.Log("Not Joined Lobby");
            return null;
        }
        if (CurrentLobby.Data == null)
        {
            Debug.Log("No Data in Lobby");
            return null;
        }
        if (!CurrentLobby.Data.TryGetValue("relayCode", out var relayCode))
        {
            Debug.Log("No Relay Code in Lobby");
            return null;
        }
        return relayCode.Value;
    }

    public async Task<string> CreateRelay(int maxConnections, bool startHost = false){
        Debug.Log("Creating Relay");
        Allocation = await RelayService.Instance.CreateAllocationAsync(maxConnections);
        var joinCode = await RelayService.Instance.GetJoinCodeAsync(Allocation.AllocationId);
        RelayServerData data = new RelayServerData(Allocation, "dtls");
        NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(data);
        if (startHost)
        {
            NetworkManager.Singleton.StartHost();
        }
        return joinCode;
    }
    public async Task JoinRelay(string joinCode, bool startClient = false){
        if (NetworkManager.Singleton.IsHost) return;
        Debug.Log("Joining Relay with code: " + joinCode);
        JoinAllocation allocation = await RelayService.Instance.JoinAllocationAsync(joinCode);
        RelayServerData data = new RelayServerData(allocation, "dtls");
        NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(data);
        if (startClient)
        {
            NetworkManager.Singleton.StartClient();
        }
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
            this.TriggerEvent(EventName.LobbyEvents.OnLobbyDataChanged, ConvertLobbyData(CurrentLobby.Data));
        };
        LobbyEventCallbacks.DataRemoved += (data) => {
            Debug.Log("Data Removed");
            this.TriggerEvent(EventName.LobbyEvents.OnLobbyDataChanged, ConvertLobbyData(CurrentLobby.Data));
        };
        LobbyEventCallbacks.DataAdded += (data) => {
            Debug.Log("Data Added");
            this.TriggerEvent(EventName.LobbyEvents.OnLobbyDataChanged, ConvertLobbyData(CurrentLobby.Data));
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
    private Dictionary<string, string> ConvertLobbyData(Dictionary<string, DataObject> data){
        Dictionary<string, string> result = new Dictionary<string, string>();
        foreach (var item in data)
        {
            result.Add(item.Key, item.Value.Value);
        }
        return result;
    }

    public async Task AddRelayCodeAsync(string relayCode)
    {
        await UpdateLobbyDataAsync("relayCode", relayCode);
    }

    public async Task UpdateLobbyDataAsync(string key, string value, bool isPrivate = false)
    {
        if (CurrentLobby == null)
            return;

        Dictionary<string, DataObject> dataCurr = CurrentLobby.Data ?? new Dictionary<string, DataObject>();
        dataCurr[key] = new DataObject(isPrivate ? DataObject.VisibilityOptions.Private : DataObject.VisibilityOptions.Public, value);

        UpdateLobbyOptions updateOptions = new UpdateLobbyOptions { Data = dataCurr };
        CurrentLobby = await LobbyService.Instance.UpdateLobbyAsync(CurrentLobby.Id, updateOptions);
    }

    public async Task UpdateSelfPlayerDataAsync(string key, string value, bool isPrivate = false)
    {
        if (CurrentLobby == null)
            return;

        Dictionary<string, PlayerDataObject> dataCurr = SelfPlayer.Data ?? new Dictionary<string, PlayerDataObject>();
        dataCurr[key] = new PlayerDataObject(isPrivate ? PlayerDataObject.VisibilityOptions.Private : PlayerDataObject.VisibilityOptions.Public, value);

        UpdatePlayerOptions updateOptions = new UpdatePlayerOptions { Data = dataCurr };
        await LobbyService.Instance.UpdatePlayerAsync(CurrentLobby.Id, SelfPlayer.Id, updateOptions);
    }

}
