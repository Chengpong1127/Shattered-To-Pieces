using Unity.Netcode;
using Cinemachine;

public class GameLocalPlayerManager: LocalPlayerManager{
    public CinemachineVirtualCamera VirtualCamera;
    public Minimap Minimap;
    private void SetCamera(){
        VirtualCamera.Follow = Player.GetTracedTransform();
        if (Minimap != null) Minimap.player = Player.GetTracedTransform();
    }

    protected override void PlayerSetup(){
        SetCamera();
        Player.AbilityActionMap.Enable();
    }
}