


using System;
using System.Collections.Generic;
using UnityEngine;

public class AssemblyRoomLocalPlayerManager: BaseLocalPlayerManager, IAssemblyRoom{
    public AssemblyController assemblyController => throw new NotImplementedException();

    public int PlayerInitMoney { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

    public AbilityManager AbilityManager => throw new NotImplementedException();

    public AbilityRunner AbilityRunner => throw new NotImplementedException();

    public IAbilityRebinder AbilityRebinder => throw new NotImplementedException();

    public event Action<AssemblyRoomMode> OnSetRoomMode;
    public event Action OnLoadedDevice;
    public event Action OnSavedDevice;
    private IGameComponentFactory _gameComponentFactory;

    public IGameComponent CreateNewGameComponent(GameComponentData componentData, Vector2 position)
    {
        throw new NotImplementedException();
    }

    public int GetDeviceTotalCost()
    {
        throw new NotImplementedException();
    }

    public List<GameComponentData> GetGameComponentDataListByTypeForShop(GameComponentType type)
    {
        throw new NotImplementedException();
    }

    public int GetPlayerRemainedMoney()
    {
        throw new NotImplementedException();
    }

    public void LoadDevice(int DeviceID)
    {
        throw new NotImplementedException();
    }

    public void SaveCurrentDevice()
    {
        throw new NotImplementedException();
    }

    public void SetRoomMode(AssemblyRoomMode mode)
    {
        throw new NotImplementedException();
    }

    protected override void PlayerSetup(){
        Player.LocalAbilityActionMap.Enable();
        _gameComponentFactory = new NetworkGameComponentFactory();
    }
}