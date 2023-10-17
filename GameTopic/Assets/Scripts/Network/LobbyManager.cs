using System.Collections.Generic;
using UnityEngine;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using Cysharp.Threading.Tasks;
using System;
using System.Linq;

public class LobbyManager
{
    // Server Events
    public event Action<Player> OnPlayerJoined;
    public event Action<Player> OnPlayerReady;
    public event Action<Player> OnPlayerUnready;

    // Client Events
    public event Action OnLobbyReady;

    public LobbyIdentity Identity { get; private set; }
    public Lobby CurrentLobby { get; private set; }
    public Player SelfPlayer { get; private set; }

    public LobbyManager(Player player){
        SelfPlayer = player;
    }
    public async UniTask<Lobby> CreateLobby(string lobbyName, int maxPlayers){
        SelfPlayer.Data = GetDefaultPlayerData();
        var createLobbyOptions = new CreateLobbyOptions(){
            IsPrivate = false,
            IsLocked = false,
            Player = SelfPlayer,
            Data = GetDefaultLobbyData()
            };
        CurrentLobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, maxPlayers, createLobbyOptions);
        await BindHostLobbyHandler(CurrentLobby);
        Identity = LobbyIdentity.Host;
        return CurrentLobby;
    }

    public string GetLobbyMap(){
        return CurrentLobby.Data["Map"].Value;
    }

    private Dictionary<string, DataObject> GetDefaultLobbyData(){
        return new Dictionary<string, DataObject>(){
            {"Ready", new DataObject(DataObject.VisibilityOptions.Public, "false")},
            {"HostNetworkInfo", new DataObject(DataObject.VisibilityOptions.Public, NetworkTool.GetLocalNetworkHost().ToJson())},
            {"Map", new DataObject(DataObject.VisibilityOptions.Public, "MapTest")},
        };
    }
    private Dictionary<string, PlayerDataObject> GetDefaultPlayerData(){
        return new Dictionary<string, PlayerDataObject>(){
            {"Ready", new PlayerDataObject(PlayerDataObject.VisibilityOptions.Public, "false")}
        };
    }

    public async UniTask<Lobby[]> GetAllAvailableLobby(){
        QueryLobbiesOptions queryLobbiesOptions = new QueryLobbiesOptions {
            Order = new List<QueryOrder> {
                new QueryOrder (field: QueryOrder.FieldOptions.Created, asc: true)
            },
            Filters = new List<QueryFilter>{
                new QueryFilter(field: QueryFilter.FieldOptions.AvailableSlots, op: QueryFilter.OpOptions.GE, value: "1")
            }
        };
        var responce = await LobbyService.Instance.QueryLobbiesAsync(queryLobbiesOptions);
        return responce.Results.ToArray();
    }

    public async UniTask JoinLobby(Lobby lobby){
        SelfPlayer.Data = GetDefaultPlayerData();
        var joinLobbyOptions = new JoinLobbyByIdOptions(){
            Player = SelfPlayer,
        };
        CurrentLobby = await LobbyService.Instance.JoinLobbyByIdAsync(lobby.Id, joinLobbyOptions);
        await BindClinetLobbyHandler(CurrentLobby);
        Identity = LobbyIdentity.Client;
    }

    private async UniTask BindHostLobbyHandler(Lobby lobby){
        LobbyEventCallbacks lobbyEventCallbacks = new LobbyEventCallbacks();
        lobbyEventCallbacks.DataChanged += DataChangedHandler;
        lobbyEventCallbacks.PlayerJoined += player => Debug.Log("Player " + player + " joined");
        lobbyEventCallbacks.PlayerDataChanged += data => HostPlayerDataChangedHandler(lobby, data);
        lobbyEventCallbacks.PlayerDataAdded += data => HostPlayerDataChangedHandler(lobby, data);
        await LobbyService.Instance.SubscribeToLobbyEventsAsync(lobby.Id, lobbyEventCallbacks);
    }

    private void HostPlayerDataChangedHandler(Lobby lobby, Dictionary<int, Dictionary<string, ChangedOrRemovedLobbyValue<PlayerDataObject>>> data)
    {
        var readyPlayers = data.Where(playerData => playerData.Value["Ready"].Value.Value == "true")
                                .Select(playerData => lobby.Players[playerData.Key]);

        readyPlayers.ToList().ForEach(player => OnPlayerReady?.Invoke(player));
        CheckLobbyReady();
    }

    private async UniTask BindClinetLobbyHandler(Lobby lobby){
        LobbyEventCallbacks lobbyEventCallbacks = new LobbyEventCallbacks();
        lobbyEventCallbacks.DataChanged += DataChangedHandler;
        await LobbyService.Instance.SubscribeToLobbyEventsAsync(lobby.Id, lobbyEventCallbacks);
    }

    private void DataChangedHandler(Dictionary<string, ChangedOrRemovedLobbyValue<DataObject>> data){
        if (data["Ready"].Value.Value == "true"){
            OnLobbyReady?.Invoke();
        }
    }

    public async void PlayerReady(){
        CurrentLobby = await LobbyService.Instance.UpdatePlayerAsync(CurrentLobby.Id, SelfPlayer.Id, new UpdatePlayerOptions { 
            Data = new Dictionary<string, PlayerDataObject> { 
                { "Ready", new PlayerDataObject(
                    PlayerDataObject.VisibilityOptions.Public, 
                    "true") 
                }
            } });
        SelfPlayer = CurrentLobby.Players.Where(player => player.Id == SelfPlayer.Id).First();
        OnPlayerReady?.Invoke(SelfPlayer);
        if (Identity == LobbyIdentity.Host)
            CheckLobbyReady();
    }

    public async void PlayerUnready(){
        CurrentLobby = await LobbyService.Instance.UpdatePlayerAsync(CurrentLobby.Id, SelfPlayer.Id, new UpdatePlayerOptions { 
            Data = new Dictionary<string, PlayerDataObject> { 
                { "Ready", new PlayerDataObject(
                    PlayerDataObject.VisibilityOptions.Public, 
                    "false") 
                }
            } });
        SelfPlayer = CurrentLobby.Players.Where(player => player.Id == SelfPlayer.Id).First();
        OnPlayerUnready?.Invoke(SelfPlayer);
    }

    public void CheckLobbyReady(){
        CurrentLobby.Players.ToList().ForEach(player => {
            if (player.Data["Ready"].Value == "false"){
                return;
            }
        });
        OnLobbyReady?.Invoke();
    }
    public string GetLobbyHostIP(){
        try{
            var networkHost = NetworkHost.FromJson(CurrentLobby.Data["HostNetworkInfo"].Value);
            var selfNetworkHost = NetworkTool.GetLocalNetworkHost();
            (string, string) resultIPs;
            if (NetworkTool.AtSameSubnet(networkHost, selfNetworkHost, out resultIPs)){
                return resultIPs.Item1;
            }
            return null;
        }catch(Exception){
            return null;
        }
    }

    public enum LobbyIdentity{
        Host,
        Client
    }
}
