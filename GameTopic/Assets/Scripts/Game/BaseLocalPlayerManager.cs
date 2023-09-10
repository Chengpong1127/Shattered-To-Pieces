using Unity.Netcode;
using UnityEngine;
using Cysharp.Threading.Tasks;
using UnityEngine.Events;

public class BaseLocalPlayerManager : NetworkBehaviour
{
    public PlayerDevice Player { get; private set; }
    public bool IsLocalPlayerCompleteSetup { get; private set; } = false;
    public UnityEvent OnLocalPlayerLoaded;
    public string InitLoadDeviceName = "0";


    [ClientRpc]
    public void LocalPlayerSetup_ClientRpc()
    {
        WaitPlayerLoaded();
    }
    private async void WaitPlayerLoaded(){
        Player = Utils.GetLocalPlayerDevice();
        Debug.Assert(Player != null, "Player is null");
        Player.LoadLocalDevice(InitLoadDeviceName);
        await UniTask.WaitUntil(() => Player.IsLoaded);
        PlayerSetup();
        IsLocalPlayerCompleteSetup = true;
        OnLocalPlayerLoaded?.Invoke();
    }
    /// <summary>
    /// This method will be invoked after the local player is loaded.
    /// </summary>
    protected virtual void PlayerSetup(){
    }
}