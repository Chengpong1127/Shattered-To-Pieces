using Unity.Netcode;
using Cinemachine;
using UnityEngine;
using System.Collections;
using Cysharp.Threading.Tasks;
using UnityEngine.InputSystem;
using System.Linq;
using AbilitySystem.Authoring;
using System;
using UnityEngine.Events;

public class LocalPlayerManager : NetworkBehaviour
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
        await UniTask.WaitUntil(() => Player.IsLoaded);
        OnLocalPlayerLoaded?.Invoke();
        PlayerSetup();
    }
    protected virtual void PlayerSetup(){
    }
}