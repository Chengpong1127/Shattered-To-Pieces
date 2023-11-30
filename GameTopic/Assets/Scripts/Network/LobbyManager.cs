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


    public event Action<PlayerLobbyReadyInfo> OnLobbyReady;
    public event Action<ILobbyChanges> OnLobbyChanged;

    public LobbyIdentity Identity { get; private set; }
    public Lobby CurrentLobby { get; private set; }
    public Player SelfPlayer { get; private set; }

    public LobbyManager(Player player){
        SelfPlayer = player;
    }
    public async UniTask<Lobby> CreateLobby(string lobbyName, MapInfo defaultMapInfo, PlayerProfile playerProfile){
        if (CurrentLobby != null){
            throw new Exception("Already in a lobby");
        }
        SelfPlayer.Data = GetDefaultPlayerData(playerProfile);
        var createLobbyOptions = new CreateLobbyOptions(){
            IsPrivate = false,
            IsLocked = false,
            Player = SelfPlayer,
            Data = GetDefaultLobbyData(defaultMapInfo.MapName)
            };
        try{
            CurrentLobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, defaultMapInfo.MapPlayerCount, createLobbyOptions);
        }catch(LobbyServiceException e){
            CurrentLobby = null;
            throw e;
        }
        await BindHostLobbyHandler(CurrentLobby);
        Identity = LobbyIdentity.Host;
        OnPlayerJoinOrLeave?.Invoke();
        LobbyHeartbeat(CurrentLobby.Id);
        return CurrentLobby;
    }

    public void HostDeleteLobby(){
        if (Identity != LobbyIdentity.Host){
            throw new Exception("Only Host can delete lobby");
        }
        LobbyService.Instance.DeleteLobbyAsync(CurrentLobby.Id);
        CurrentLobby = null;
    }

    public async UniTask<Lobby> ChangeLobbyMap(MapInfo mapInfo){
        Debug.Assert(Identity == LobbyIdentity.Host, "Only Host can change map");
        CurrentLobby = await LobbyService.Instance.UpdateLobbyAsync(CurrentLobby.Id, new UpdateLobbyOptions { 
            MaxPlayers = mapInfo.MapPlayerCount,
            Data = new Dictionary<string, DataObject> {
                { "MapName", new DataObject(
                    DataObject.VisibilityOptions.Public, 
                    mapInfo.MapName) 
                }
            } });
        return CurrentLobby;
    }
    private async void LobbyHeartbeat(string lobbyId){
        while(CurrentLobby != null && CurrentLobby.Id == lobbyId){
            await LobbyService.Instance.SendHeartbeatPingAsync(lobbyId);
            await UniTask.WaitForSeconds(15);
        }
    }

    public async void LeaveLobby(){
        if (Identity == LobbyIdentity.Host){
            await LobbyService.Instance.DeleteLobbyAsync(CurrentLobby.Id);
        }else{
            await LobbyService.Instance.RemovePlayerAsync(CurrentLobby.Id, SelfPlayer.Id);
        }
        CurrentLobby = null;
    }

    public PlayerProfile GetPlayerProfile(Player player){
        return PlayerProfile.FromJson(player.Data["PlayerProfileJson"].Value);
    }


    private Dictionary<string, DataObject> GetDefaultLobbyData(string mapName){
        return new Dictionary<string, DataObject>(){
            {"Ready", new DataObject(DataObject.VisibilityOptions.Public, "false")},
            {"Gaming", new DataObject(DataObject.VisibilityOptions.Public, "false")},
            {"HostNetworkInfo", new DataObject(DataObject.VisibilityOptions.Public, NetworkTool.GetLocalNetworkHost().ToJson())},
            {"MapName", new DataObject(DataObject.VisibilityOptions.Public, mapName)},
        };
    }
    private Dictionary<string, PlayerDataObject> GetDefaultPlayerData(PlayerProfile profile){
        return new Dictionary<string, PlayerDataObject>(){
            {"Ready", new PlayerDataObject(PlayerDataObject.VisibilityOptions.Public, "false")},
            {"PlayerProfileJson", new PlayerDataObject(PlayerDataObject.VisibilityOptions.Public, profile.ToJson())},
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
        var response = await LobbyService.Instance.QueryLobbiesAsync(queryLobbiesOptions);
        var lobbies = response.Results;
        lobbies = lobbies.Where(lobby => GetLobbyHostIP(lobby) != null).ToList();
        return lobbies.ToArray();
    }

    public async UniTask JoinLobby(Lobby lobby, PlayerProfile playerProfile){
        if (CurrentLobby != null){
            throw new Exception("Already in a lobby");
        }
        SelfPlayer.Data = GetDefaultPlayerData(playerProfile);
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
        lobbyEventCallbacks.LobbyChanged += OnLobbyChangedHandler;
        lobbyEventCallbacks.DataAdded += _ => DataChangedHandler();
        lobbyEventCallbacks.DataChanged += _ => DataChangedHandler();
        lobbyEventCallbacks.DataRemoved += _ => DataChangedHandler();
        lobbyEventCallbacks.PlayerJoined += _ => PlayerJoinOrLeaveHandler();
        lobbyEventCallbacks.PlayerLeft += _ => PlayerJoinOrLeaveHandler();
        lobbyEventCallbacks.PlayerDataChanged += PlayerDataChangedHandler;
        lobbyEventCallbacks.PlayerDataAdded += PlayerDataChangedHandler;
        await LobbyService.Instance.SubscribeToLobbyEventsAsync(lobby.Id, lobbyEventCallbacks);
    }
    private void OnLobbyChangedHandler(ILobbyChanges lobbyChanges){
        if (!lobbyChanges.LobbyDeleted){
            lobbyChanges.ApplyToLobby(CurrentLobby);
        }
        OnLobbyChanged?.Invoke(lobbyChanges);
    }

    public async void StartGame(){
        if (Identity != LobbyIdentity.Host){
            throw new Exception("Only Host can set starting game");
        }
        await LobbyService.Instance.UpdateLobbyAsync(CurrentLobby.Id, new UpdateLobbyOptions { 
            Data = new Dictionary<string, DataObject> { 
                { "Gaming", new DataObject(
                    DataObject.VisibilityOptions.Public, 
                    "true") 
                }
            } });
    }

    private void PlayerJoinOrLeaveHandler(){
        if (CurrentLobby.HostId == SelfPlayer.Id){
            Identity = LobbyIdentity.Host;
        } else {
            Identity = LobbyIdentity.Client;
        }
        OnPlayerJoinOrLeave?.Invoke();
    }

    private void PlayerDataChangedHandler(Dictionary<int, Dictionary<string, ChangedOrRemovedLobbyValue<PlayerDataObject>>> data)
    {
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
        lobbyEventCallbacks.LobbyChanged += OnLobbyChangedHandler;
        lobbyEventCallbacks.DataAdded += _ => DataChangedHandler();
        lobbyEventCallbacks.DataChanged += _ => DataChangedHandler();
        lobbyEventCallbacks.DataRemoved += _ => DataChangedHandler();
        lobbyEventCallbacks.PlayerJoined += _ => PlayerJoinOrLeaveHandler();
        lobbyEventCallbacks.PlayerLeft += _ => PlayerJoinOrLeaveHandler();
        lobbyEventCallbacks.PlayerDataAdded += PlayerDataChangedHandler;
        lobbyEventCallbacks.PlayerDataChanged += PlayerDataChangedHandler;
        try{
            await LobbyService.Instance.SubscribeToLobbyEventsAsync(lobby.Id, lobbyEventCallbacks);
        }catch(LobbyServiceException e){
            Debug.Log(e.Message);
        }
    }

    private void DataChangedHandler(){
        if (CurrentLobby.Data["Ready"].Value == "true"){
            var lobbyReadyInfo = new PlayerLobbyReadyInfo(){
                Identity = Identity,
                Player = SelfPlayer,
                HostAddress = GetLobbyHostIP(CurrentLobby),
                MapName = CurrentLobby.Data["MapName"].Value
            };
            OnLobbyReady?.Invoke(lobbyReadyInfo);
        }
    }

    public async UniTask PlayerReady(){
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

    public async UniTask PlayerUnready(){
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
        if (!isAllReady || CurrentLobby.Players.Count < CurrentLobby.MaxPlayers){
            return;
        }
        CurrentLobby = await LobbyService.Instance.UpdateLobbyAsync(CurrentLobby.Id, new UpdateLobbyOptions { 
            Data = new Dictionary<string, DataObject> { 
                { "Ready", new DataObject(
                    DataObject.VisibilityOptions.Public, 
                    "true") 
                }
            } });
        var lobbyReadyInfo = new PlayerLobbyReadyInfo(){
            Identity = Identity,
            Player = SelfPlayer,
            HostAddress = GetLobbyHostIP(CurrentLobby),
            MapName = CurrentLobby.Data["MapName"].Value
        };
        OnLobbyReady?.Invoke(lobbyReadyInfo);
    }
    private string GetLobbyHostIP(Lobby lobby){
        try{
            var networkHost = NetworkHost.FromJson(lobby.Data["HostNetworkInfo"].Value);
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
