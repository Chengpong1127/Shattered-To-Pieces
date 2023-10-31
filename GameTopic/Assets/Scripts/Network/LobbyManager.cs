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
    public event Action OnPlayerJoinOrLeave;
    public event Action<Player> OnPlayerReady;
    public event Action<Player> OnPlayerUnready;

    // Client Events
    public event Action<PlayerLobbyReadyInfo> OnLobbyReady;

    public LobbyIdentity Identity { get; private set; }
    public Lobby CurrentLobby { get; private set; }
    public Player SelfPlayer { get; private set; }

    public LobbyManager(Player player){
        SelfPlayer = player;
    }
    public async UniTask<Lobby> CreateLobby(string lobbyName, MapInfo defaultMapInfo){
        SelfPlayer.Data = GetDefaultPlayerData();
        var createLobbyOptions = new CreateLobbyOptions(){
            IsPrivate = false,
            IsLocked = false,
            Player = SelfPlayer,
            Data = GetDefaultLobbyData(defaultMapInfo.MapName)
            };
        CurrentLobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, defaultMapInfo.MapPlayerCount, createLobbyOptions);
        await BindHostLobbyHandler(CurrentLobby);
        Identity = LobbyIdentity.Host;
        OnPlayerJoinOrLeave?.Invoke();
        return CurrentLobby;
    }

    public async UniTask<Lobby> ChangeLobbyMap(MapInfo mapInfo){
        CurrentLobby = await LobbyService.Instance.UpdateLobbyAsync(CurrentLobby.Id, new UpdateLobbyOptions { 
            Data = new Dictionary<string, DataObject> { 
                { "MapName", new DataObject(
                    DataObject.VisibilityOptions.Public, 
                    mapInfo.MapName) 
                }
            } });
        return CurrentLobby;
    }


    private Dictionary<string, DataObject> GetDefaultLobbyData(string mapName){
        return new Dictionary<string, DataObject>(){
            {"Ready", new DataObject(DataObject.VisibilityOptions.Public, "false")},
            {"HostNetworkInfo", new DataObject(DataObject.VisibilityOptions.Public, NetworkTool.GetLocalNetworkHost().ToJson())},
            {"MapName", new DataObject(DataObject.VisibilityOptions.Public, mapName)},
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
        OnPlayerJoinOrLeave?.Invoke();
    }

    private async UniTask BindHostLobbyHandler(Lobby lobby){
        LobbyEventCallbacks lobbyEventCallbacks = new LobbyEventCallbacks();
        lobbyEventCallbacks.DataChanged += DataChangedHandler;
        lobbyEventCallbacks.PlayerJoined += async (joinedPlayerList) => {
            CurrentLobby = await LobbyService.Instance.GetLobbyAsync(CurrentLobby.Id);
            OnPlayerJoinOrLeave?.Invoke();
        };
        lobbyEventCallbacks.PlayerJoined += async (joinedPlayerList) => {
            CurrentLobby = await LobbyService.Instance.GetLobbyAsync(CurrentLobby.Id);
            OnPlayerJoinOrLeave?.Invoke();
        };
        lobbyEventCallbacks.PlayerDataChanged += PlayerDataChangedHandler;
        lobbyEventCallbacks.PlayerDataAdded += PlayerDataChangedHandler;
        await LobbyService.Instance.SubscribeToLobbyEventsAsync(lobby.Id, lobbyEventCallbacks);
    }

    private async void PlayerDataChangedHandler(Dictionary<int, Dictionary<string, ChangedOrRemovedLobbyValue<PlayerDataObject>>> data)
    {
        CurrentLobby = await LobbyService.Instance.GetLobbyAsync(CurrentLobby.Id);
        foreach(var (key, value) in data)
        {
            var player = CurrentLobby.Players[key];
            var playerData = value;
            if (player.Id == SelfPlayer.Id)
                continue;
            if (playerData["Ready"].Value.Value == "true"){
                OnPlayerReady?.Invoke(CurrentLobby.Players[key]);
            }
            if (playerData["Ready"].Value.Value == "false"){
                OnPlayerUnready?.Invoke(CurrentLobby.Players[key]);
            }
        }
        if (Identity == LobbyIdentity.Host)
            CheckLobbyReady();
    }

    private async UniTask BindClinetLobbyHandler(Lobby lobby){
        LobbyEventCallbacks lobbyEventCallbacks = new LobbyEventCallbacks();
        lobbyEventCallbacks.DataChanged += DataChangedHandler;
        lobbyEventCallbacks.PlayerJoined += async (joinedPlayerList) => {
            CurrentLobby = await LobbyService.Instance.GetLobbyAsync(CurrentLobby.Id);
            OnPlayerJoinOrLeave?.Invoke();
        };
        lobbyEventCallbacks.PlayerJoined += async (joinedPlayerList) => {
            CurrentLobby = await LobbyService.Instance.GetLobbyAsync(CurrentLobby.Id);
            OnPlayerJoinOrLeave?.Invoke();
        };
        lobbyEventCallbacks.PlayerDataAdded += PlayerDataChangedHandler;
        lobbyEventCallbacks.PlayerDataChanged += PlayerDataChangedHandler;
        await LobbyService.Instance.SubscribeToLobbyEventsAsync(lobby.Id, lobbyEventCallbacks);
    }

    private void DataChangedHandler(Dictionary<string, ChangedOrRemovedLobbyValue<DataObject>> data){
        if (data["Ready"].Value.Value == "true"){
            var lobbyReadyInfo = new PlayerLobbyReadyInfo(){
                Identity = Identity,
                Player = SelfPlayer,
                HostAddress = GetLobbyHostIP(),
                MapName = CurrentLobby.Data["MapName"].Value
            };
            OnLobbyReady?.Invoke(lobbyReadyInfo);
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

    public async void CheckLobbyReady(){
        bool isAllReady = true;
        CurrentLobby.Players.ToList().ForEach(player => {
            if (player.Data["Ready"].Value == "false"){
                isAllReady = false;
            }
        });
        if (!isAllReady){
            return;
        }
        await LobbyService.Instance.UpdateLobbyAsync(CurrentLobby.Id, new UpdateLobbyOptions { 
            Data = new Dictionary<string, DataObject> { 
                { "Ready", new DataObject(
                    DataObject.VisibilityOptions.Public, 
                    "true") 
                }
            } });
        var lobbyReadyInfo = new PlayerLobbyReadyInfo(){
            Identity = Identity,
            Player = SelfPlayer,
            HostAddress = GetLobbyHostIP(),
            MapName = CurrentLobby.Data["MapName"].Value
        };
        OnLobbyReady?.Invoke(lobbyReadyInfo);
    }
    private string GetLobbyHostIP(){
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

    public class PlayerLobbyReadyInfo{
        public LobbyIdentity Identity;
        public Player Player;
        public string HostAddress;
        public string MapName;
    }
}
