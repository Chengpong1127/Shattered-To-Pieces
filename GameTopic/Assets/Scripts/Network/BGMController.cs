using DigitalRuby.SoundManagerNamespace;
using UnityEngine;

[RequireComponent(typeof(AudioSource), typeof(LocalGameManager))]
public class BGMController : MonoBehaviour
{
    [SerializeField] public AudioClip homeBGM;
    [SerializeField] [Range(0, 1)] public float homeBGMVolume = 1;
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
                audioSource.PlayLoopingMusicManaged(homeBGMVolume, 1, false);
                break;
            case LocalGameManager.GameState.GameRoom:
                var info = localGameManager.CurrentMapInfo;
                if (info.BackgroundMusic == null) return;
                audioSource.Stop();
                audioSource.clip = info.BackgroundMusic;
                audioSource.PlayLoopingMusicManaged(info.BackgroundMusicVolume, 1, false);
                break;
        }
    }
}