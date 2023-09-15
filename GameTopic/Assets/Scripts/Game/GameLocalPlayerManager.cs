using Unity.Netcode;
using Cinemachine;
using UnityEngine;

public class GameLocalPlayerManager: BaseLocalPlayerManager{
    public CinemachineVirtualCamera VirtualCamera;
    public Minimap Minimap;
    private void SetCamera(){
        VirtualCamera.Follow = Player.GetTracedTransform();
        if (Minimap != null) Minimap.player = Player.GetTracedTransform();
    }

    protected override void PlayerSpawnSetup(){
        Debug.Log("PlayerSetup");
        SetCamera();
        Player.LocalAbilityActionMap.Enable();
    }
}