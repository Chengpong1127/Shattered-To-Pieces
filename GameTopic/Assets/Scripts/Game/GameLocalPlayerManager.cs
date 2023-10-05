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

    private void Loading_Exit(){
        SetCamera();
        Player.LocalAbilityActionMap.Enable();
    }
}