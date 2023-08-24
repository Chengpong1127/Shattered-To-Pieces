using System.Data.SqlTypes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using Newtonsoft.Json;
using UnityEngine.InputSystem;
using Unity.VisualScripting;

public class PlayerDevice : NetworkBehaviour
{
    private Device device;
    private IGameComponentFactory factory;
    private PlayerInput playerInput;
    private Dictionary<InputAction, int> actionToAbilityNumber = new Dictionary<InputAction, int>();
    private AbilityRunner abilityRunner;
    void Awake()
    {
        
    }
    [Rpc(sources: RpcSources.InputAuthority, targets: RpcTargets.StateAuthority)]
    public void LoadDevice_RPC(string infoData){
        device = new Device(factory);
        var info = JsonConvert.DeserializeObject<DeviceInfo>(infoData);
        device.Load(info);
        abilityRunner = AbilityRunner.CreateInstance(gameObject, device.AbilityManager);
    }
    public override void Spawned()
    {
        factory = new NetworkGameComponentFactory(Runner);
        if (HasInputAuthority){
            var info = ResourceManager.Instance.LoadLocalDeviceInfo("0") ?? ResourceManager.Instance.LoadDefaultDeviceInfo();
            string infoData = JsonConvert.SerializeObject(info);
            LoadDevice_RPC(infoData);

            
            playerInput = GetComponent<PlayerInput>();
            SetInputActionMap(playerInput.currentActionMap, info.abilityManagerInfo.EntryPaths);
        }
        
    }
    public override void FixedUpdateNetwork()
    {
        if (HasStateAuthority){
            if (GetInput<NetworkInputData>(out var inputData)){
                for(int i = 0; i < 10; i++){
                    if (inputData.StartAbilityEntry.IsSet(i)){
                        abilityRunner.StartAbility(i);
                    }
                }
            }
        }
    }

    private void SetInputActionMap(InputActionMap map, string[] keyPaths){
        map.Disable();
        for(int i = 0; i < keyPaths.Length; i++){
            string keyName = "Ability" + i.ToString();
            InputAction action = map.FindAction(keyName) ?? map.AddAction(keyName);
            action.AddBinding(keyPaths[i]);
            actionToAbilityNumber.Add(action, i);
            action.started += StartAbility;
        }
        map.Enable();
    }
    private void StartAbility(InputAction.CallbackContext ctx){
        if (actionToAbilityNumber.TryGetValue(ctx.action, out int abilityNumber)){
            this.TriggerEvent(EventName.AbilityManagerEvents.OnAbilityTriggered, abilityNumber);
        }
    }
}
public struct NetworkInputData : INetworkInput
{
    public NetworkButtons StartAbilityEntry;
}