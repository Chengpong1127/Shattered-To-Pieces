using UnityEngine;
using Unity.Netcode;
using System.Linq;
using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;

public class AssemblyRoomRunner: GameRunner{
    [SerializeField]
    private Transform _spawnPoint;
    public GamePlayer ControlledPlayer => PlayerMap.Values.First() as GamePlayer;
    public int PlayerInitMoney = 200;
    public event Action<IGameComponent> OnBuyingGameComponent;
    public event Action<int> OnMoneyChanged;

    public int GetPlayerRemainedMoney()
    {
        return PlayerInitMoney - GetTotalCost();
    }
    public event Action OnLoadedDevice;
    public event Action OnSavedDevice;
    private IGameComponentFactory _gameComponentFactory;
    public List<GameComponentData> GameComponentDataList {
        get {
            _gameComponentDataList ??= ResourceManager.Instance.LoadAllGameComponentData();
            return _gameComponentDataList;
        }
    }
    private List<GameComponentData> _gameComponentDataList;
    private List<GameComponent> _allGameComponents = new List<GameComponent>();
    protected override void Awake() {
        base.Awake();
        GameEvents.GameComponentEvents.OnEntityDied += OnEntityDiedHandler;
    }
    private void OnEntityDiedHandler(BaseEntity entity){
        if (entity is GameComponent component){
            _allGameComponents.Remove(component);
            OnMoneyChanged?.Invoke(GetPlayerRemainedMoney());
        }
    }
    public override void OnDestroy() {
        base.OnDestroy();
        GameEvents.GameComponentEvents.OnEntityDied -= OnEntityDiedHandler;
    }
    public int GetTotalCost()
    {
        var allGameComponentsCost = _allGameComponents
            .Sum(component => GameComponentDataList
                .Where(data => data.ResourcePath == component.ComponentName)
                .Select(data => data.Price)
                .FirstOrDefault());
        return allGameComponentsCost;
    }
    public void SaveCurrentDevice()
    {
        if (ControlledPlayer.SelfDevice == null) return;
        var info = ControlledPlayer.SelfDevice.Dump();
        ResourceManager.Instance.SaveLocalDeviceInfo(info as DeviceInfo, CurrentDeviceID.ToString());
        OnSavedDevice?.Invoke();
        GameEvents.AssemblyRoomEvents.OnSavedDevice.Invoke();
    }

    public List<GameComponentData> GetGameComponentDataListByTypeForShop(GameComponentType type)
    {
        var filteredList = GameComponentDataList.Where((data) => data.Type == type && data.DisplayAtShop == true).ToList();
        return filteredList;
    }
    public int CurrentDeviceID { get; private set; } = 0;

    private void Loading_Enter(){
        _gameComponentFactory = new NetworkGameComponentFactory();
    }


    protected override void GameStartSpawnAllPlayer()
    {
        LoadDevice(CurrentDeviceID);
    }

    public void CleanAllGameComponents(){
        _allGameComponents.ForEach(component => {
            component.Die();
        });
        _allGameComponents.Clear();
        OnMoneyChanged?.Invoke(GetPlayerRemainedMoney());
    }

    public override void SpawnDevice(BasePlayer player, string filename){
        CleanAllGameComponents();
        player.ServerLoadDevice(filename, _spawnPoint.position);
    }

    public IGameComponent CreateNewGameComponent(GameComponentData componentData, Vector2 position)
    {
        IGameComponent component = _gameComponentFactory.CreateGameComponentObject(componentData.ResourcePath, position);
        OnBuyingGameComponent?.Invoke(component);
        _allGameComponents.Add(component as GameComponent);
        OnMoneyChanged?.Invoke(GetPlayerRemainedMoney());
        return component;
    }

    public async void LoadDevice(int DeviceID)
    {
        SaveCurrentDevice();
        SpawnDevice(ControlledPlayer, DeviceID.ToString());
        ControlledPlayer.LocalAbilityActionMap.Enable();
        CurrentDeviceID = DeviceID;
        OnLoadedDevice?.Invoke();
        GameEvents.AssemblyRoomEvents.OnLoadedDevice.Invoke();
        await UniTask.WaitUntil(() => ControlledPlayer.IsAlive.Value);
        _allGameComponents.AddRange(ControlledPlayer.SelfDevice.GetAllGameComponents());
        OnMoneyChanged?.Invoke(GetPlayerRemainedMoney());
    }
    private void OnApplicationQuit() {
        SaveCurrentDevice();
    }
}
public enum GameComponentType{
    Basic,
    Attack,
    Movement,
    Functional
}   