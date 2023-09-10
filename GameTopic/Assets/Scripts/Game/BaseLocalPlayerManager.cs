using Unity.Netcode;
using UnityEngine;
using Cysharp.Threading.Tasks;
using UnityEngine.Events;

public class BaseLocalPlayerManager : NetworkBehaviour
{
    public PlayerDevice Player { get; private set; }
    public UnityEvent OnLocalPlayerLoaded;



    [ClientRpc]
    public void LocalPlayerSetup_ClientRpc()
    {
        WaitPlayerLoaded();
    }
    private async void WaitPlayerLoaded(){
        Player = Utils.GetLocalPlayerDevice();
        Debug.Assert(Player != null, "Player is null");
        Player.LoadLocalDevice("0");
        await UniTask.WaitUntil(() => Player.IsLoaded);
        OnLocalPlayerLoaded?.Invoke();
        PlayerSetup();
    }
    /// <summary>
    /// This method will be invoked after the local player is loaded.
    /// </summary>
    protected virtual void PlayerSetup(){
    }
}