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

public class LobbyManager
{
    public Lobby CurrentLobby;
    public Player SelfPlayer;
    public Allocation Allocation;
    public async Task SignIn(){
        await UnityServices.InitializeAsync();
        AuthenticationService.Instance.SignedIn += () => {
            Debug.Log("Signed In with ID: " + AuthenticationService.Instance.PlayerId);
        };
        await AuthenticationService.Instance.SignInAnonymouslyAsync();
        SelfPlayer = new Player(AuthenticationService.Instance.PlayerId);
    }
    public async Task CreateLobby(string lobbyName, int maxPlayers, CreateLobbyOptions options = null, bool createRelay = false){
        var createLobbyOptions = options ?? new CreateLobbyOptions(){
            IsPrivate = false,
            IsLocked = false,
            Player = SelfPlayer};
        CurrentLobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, maxPlayers, createLobbyOptions);
        BindLobbyEvents(CurrentLobby.Id);
        Debug.Log("Lobby Created with Lobby ID: " + CurrentLobby.Id);

        if (createRelay)
        {
            string relayCode = await CreateRelay(maxPlayers);
            Debug.Log("Relay Code: " + relayCode);
            await AddRelayCodeAsync(relayCode);
            NetworkManager.Singleton.StartHost();
        }
    }

    public async Task<Lobby> GetTheLastestLobby(){
        QueryLobbiesOptions queryLobbiesOptions = new QueryLobbiesOptions {
            Order = new List<QueryOrder> {
                new QueryOrder (field: QueryOrder.FieldOptions.Created, asc: true)
            },
            Filters = new List<QueryFilter>{
                new QueryFilter(field: QueryFilter.FieldOptions.AvailableSlots, op: QueryFilter.OpOptions.GE, value: "1")
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

    public async Task JoinLobby(Lobby lobby, bool joinRelay = false){
        if (CurrentLobby != null)
        {
            Debug.Log("Already Joined Lobby");
            return;
        }
        JoinLobbyByIdOptions quickJoinLobbyOptions = new JoinLobbyByIdOptions { Player = SelfPlayer };
        CurrentLobby = await LobbyService.Instance.JoinLobbyByIdAsync(lobby.Id, quickJoinLobbyOptions);
        BindLobbyEvents(CurrentLobby.Id);
        Debug.Log("Joined new lobby with ID: " + CurrentLobby.Id);

        if (joinRelay)
        {
            string relayCode = GetLobbyRelayCode();
            await JoinRelay(relayCode);
            NetworkManager.Singleton.StartClient();
        }
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
        if (!CurrentLobby.Data.TryGetValue("RelayCode", out var relayCode))
        {
            Debug.Log("No Relay Code in Lobby");
            return null;
        }
        return relayCode.Value;
    }

    private async Task<string> CreateRelay(int maxConnections){
        Debug.Log("Creating Relay");
        Allocation = await RelayService.Instance.CreateAllocationAsync(maxConnections);
        var joinCode = await RelayService.Instance.GetJoinCodeAsync(Allocation.AllocationId);
        RelayServerData data = new RelayServerData(Allocation, "dtls");
        NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(data);
        return joinCode;
    }
    private async Task JoinRelay(string joinCode){
        if (NetworkManager.Singleton.IsHost) return;
        Debug.Log("Joining Relay with code: " + joinCode);
        JoinAllocation allocation = await RelayService.Instance.JoinAllocationAsync(joinCode);
        RelayServerData data = new RelayServerData(allocation, "dtls");
        NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(data);
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
            GameEvents.LobbyEvents.OnLobbyDataChanged.Invoke(ConvertLobbyData(CurrentLobby.Data));
        };
        LobbyEventCallbacks.DataRemoved += (data) => {
            Debug.Log("Data Removed");
            GameEvents.LobbyEvents.OnLobbyDataChanged.Invoke(ConvertLobbyData(CurrentLobby.Data));
        };
        LobbyEventCallbacks.DataAdded += (data) => {
            Debug.Log("Data Added");
            GameEvents.LobbyEvents.OnLobbyDataChanged.Invoke(ConvertLobbyData(CurrentLobby.Data));
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

    private async Task AddRelayCodeAsync(string relayCode)
    {
        await UpdateLobbyDataAsync("RelayCode", relayCode);
    }

    private async Task UpdateLobbyDataAsync(string key, string value, bool isPrivate = false)
    {
        if (CurrentLobby == null)
            return;

        Dictionary<string, DataObject> dataCurr = CurrentLobby.Data ?? new Dictionary<string, DataObject>();
        dataCurr[key] = new DataObject(isPrivate ? DataObject.VisibilityOptions.Private : DataObject.VisibilityOptions.Public, value);

        UpdateLobbyOptions updateOptions = new UpdateLobbyOptions { Data = dataCurr };
        CurrentLobby = await LobbyService.Instance.UpdateLobbyAsync(CurrentLobby.Id, updateOptions);
    }

    private async Task UpdateSelfPlayerDataAsync(string key, string value, bool isPrivate = false)
    {
        if (CurrentLobby == null)
            return;

        Dictionary<string, PlayerDataObject> dataCurr = SelfPlayer.Data ?? new Dictionary<string, PlayerDataObject>();
        dataCurr[key] = new PlayerDataObject(isPrivate ? PlayerDataObject.VisibilityOptions.Private : PlayerDataObject.VisibilityOptions.Public, value);

        UpdatePlayerOptions updateOptions = new UpdatePlayerOptions { Data = dataCurr };
        await LobbyService.Instance.UpdatePlayerAsync(CurrentLobby.Id, SelfPlayer.Id, updateOptions);
    }

}
