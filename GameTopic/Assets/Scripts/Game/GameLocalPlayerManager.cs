using Unity.Netcode;
using Cinemachine;
using UnityEngine;
using Cysharp.Threading.Tasks;

public class GameLocalPlayerManager: LocalPlayerManager{
    public CinemachineVirtualCamera VirtualCamera;

    [SerializeField] EndGameUI EndGame;

    private void SetCamera(){
        VirtualCamera.Follow = Player.GetTracedTransform();
    }
    private void Loading_Exit(){
        SetCamera();
    }

    protected override async void GameOverHandler(GameResult result)
    {
        int rank = result.PlayerRankMap[Player.OwnerClientId];
        Debug.Log($"GameOver. You are at rank: {rank}.");

        EndGame.gameObject.SetActive(true);
        EndGame.animator.SetTrigger(rank == 1 ? "Win" : "Lose");

        await UniTask.WaitForSeconds(5);
        ExitGame();
    }
}