using Unity.Netcode;
using Cinemachine;
using UnityEngine;
using Cysharp.Threading.Tasks;
using DigitalRuby.SoundManagerNamespace;

public class GameLocalPlayerManager: LocalPlayerManager{
    public CinemachineVirtualCamera VirtualCamera;

    [SerializeField] EndGameUI EndGame;
    [SerializeField] AudioClip WinMusic;
    [SerializeField] AudioClip LoseMusic;
    [SerializeField] AudioSource audioSource;

    private async void SetCamera(){
        await UniTask.WaitUntil(() => Player.GetTracedTransform() != null);
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
        if (audioSource != null)
        {
            audioSource.Stop();
            audioSource.PlayOneShotMusicManaged(rank == 1 ? WinMusic : LoseMusic);
        }

        await UniTask.WaitForSeconds(5);
        ExitGame();
    }
}