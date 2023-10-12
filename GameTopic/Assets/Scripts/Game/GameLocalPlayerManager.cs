using Unity.Netcode;
using Cinemachine;
using UnityEngine;
using Cysharp.Threading.Tasks;

public class GameLocalPlayerManager: BaseLocalPlayerManager{
    public CinemachineVirtualCamera VirtualCamera;
    public Minimap Minimap;
    private void SetCamera(){
        VirtualCamera.Follow = Player.GetTracedTransform();
        if (Minimap != null) Minimap.player = Player.GetTracedTransform();
    }
    private void Loading_Exit(){
        SetCamera();
    }

    protected override async void GameOverHandler(GameResult result)
    {
        int rank = result.PlayerRankMap[Player.OwnerClientId];
        Debug.Log($"GameOver. You are at rank: {rank}.");
        await UniTask.WaitForSeconds(5);
        ExitGame();
    }
}