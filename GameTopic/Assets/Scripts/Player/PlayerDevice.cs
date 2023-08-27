using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Newtonsoft.Json;
using UnityEngine.InputSystem;

public class PlayerDevice : NetworkBehaviour
{
    public Device SelfDevice { get; private set; }
    public IGameComponentFactory GameComponentFactory { get; private set; }
    private AbilityRunner abilityRunner;
    private Dictionary<InputAction, int> actionToAbilityNumber = new();
    [ServerRpc]
    private void LoadDeviceServerRpc(string json)
    {
        SelfDevice = new Device(GameComponentFactory);
        SelfDevice.Load(DeviceInfo.CreateFromJson(json));
        abilityRunner = AbilityRunner.CreateInstance(gameObject, SelfDevice.AbilityManager);
    }
    [ServerRpc]
    private void StartAbility_ServerRPC(int abilityNumber)
    {
        Debug.Log("StartAbility_ServerRPC: " + abilityNumber);
        abilityRunner.StartAbility(abilityNumber);
    }
    void Awake()
    {
        GameComponentFactory = new NetworkGameComponentFactory();
    }
    void Start()
    {
        
        if (IsOwner){
            DeviceInfo info = GetLocalDeviceInfo();
            LoadDeviceServerRpc(info.ToJson());
            SetInputActionMap(new InputActionMap("AbilityAction"), info.AbilityManagerInfo.EntryPaths);
        }
        
    }
    private void SetInputActionMap(InputActionMap map, string[] keyPaths){
        map.Disable();
        for(int i = 0; i < keyPaths.Length; i++){
            string keyName = "Ability" + i.ToString();
            InputAction action = map.AddAction(keyName);
            action.AddBinding(keyPaths[i]);
            actionToAbilityNumber.Add(action, i);
            action.started += StartAbility;
        }
        map.Enable();
    }
    private void StartAbility(InputAction.CallbackContext ctx){
        if (actionToAbilityNumber.TryGetValue(ctx.action, out int abilityNumber)){
            StartAbility_ServerRPC(abilityNumber);
        }
    }

    private DeviceInfo GetLocalDeviceInfo(){
        return ResourceManager.Instance.LoadLocalDeviceInfo("0") ?? ResourceManager.Instance.LoadDefaultDeviceInfo();
    }
}
