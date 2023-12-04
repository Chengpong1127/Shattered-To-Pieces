using DigitalRuby.SoundManagerNamespace;
using UnityEngine;

[RequireComponent(typeof(AudioSource), typeof(LocalGameManager))]
public class BGMController : MonoBehaviour
{
    [SerializeField]
    private AudioClip homeBGM;
    [SerializeField]
    private AudioClip gameRoomBGM;
    private AudioSource audioSource;
    private LocalGameManager localGameManager;
    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        localGameManager = GetComponent<LocalGameManager>();
        localGameManager.StateMachine.Changed += StateMachineOnChanged;
    }

    private void StateMachineOnChanged(LocalGameManager.GameState state)
    {
        switch (state)
        {
            case LocalGameManager.GameState.Home:
                if (homeBGM == null) return;
                audioSource.Stop();
                audioSource.clip = homeBGM;
                audioSource.PlayLoopingMusicManaged();
                break;
            case LocalGameManager.GameState.GameRoom:
                if (gameRoomBGM == null) return;
                audioSource.Stop();
                audioSource.clip = gameRoomBGM;
                audioSource.PlayLoopingMusicManaged();
                break;
        }
    }
}