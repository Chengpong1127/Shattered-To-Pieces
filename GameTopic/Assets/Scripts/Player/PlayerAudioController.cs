using UnityEngine;
using Unity.Netcode;
using DigitalRuby.SoundManagerNamespace;

[RequireComponent(typeof(AudioSource))]
public class PlayerAudioController : NetworkBehaviour
{
    private AudioSource audioSource;
    [SerializeField] private AudioClip StartAbilityClip;
    [SerializeField] [Range(0, 1)] private float StartAbilityVolume = 1;
    [SerializeField] private AudioClip CancelAbilityClip;
    [SerializeField] [Range(0, 1)] private float CancelAbilityVolume = 1;
    [SerializeField] private AudioClip StartRebindClip;
    [SerializeField] [Range(0, 1)] private float StartRebindVolume = 1;
    [SerializeField] private AudioClip EndRebindClip;
    [SerializeField] [Range(0, 1)] private float EndRebindVolume = 1;

    [SerializeField] private AudioClip AbilitySelectedClip;
    [SerializeField] [Range(0, 1)] private float AbilitySelectedVolume = 1;
    [SerializeField] private AudioClip AbilitySelectedEndClip;
    [SerializeField] [Range(0, 1)] private float AbilitySelectedEndVolume = 1;

    [SerializeField] private AudioClip GameComponentSelectedClip;
    [SerializeField] [Range(0, 1)] private float GameComponentSelectedVolume = 1;
    [SerializeField] private AudioClip GameComponentConnectedClip;
    [SerializeField] [Range(0, 1)] private float GameComponentConnectedVolume = 1;
    [SerializeField] private AudioClip GameComponentDisconnectedClip;
    [SerializeField] [Range(0, 1)] private float GameComponentDisconnectedVolume = 1;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        audioSource = GetComponent<AudioSource>();
        if (IsServer)
        {
            GameEvents.GameComponentEvents.OnGameComponentSelected += GameComponentSelectedHandler;
            GameEvents.GameComponentEvents.OnGameComponentConnected += GameComponentConnectedHandler;
            GameEvents.GameComponentEvents.OnGameComponentDisconnected += GameComponentDisconnectedHandler;
        }
        if (IsOwner)
        {
            GameEvents.UIEvents.OnGameComponentAbilitySelected += GameComponentAbilitySelectedHandler;
            GameEvents.UIEvents.OnGameComponentAbilitySelectedEnd += GameComponentAbilitySelectedEndHandler;
            GameEvents.RebindEvents.OnStartRebinding += StartRebindingHandler;
            GameEvents.RebindEvents.OnFinishRebinding += FinishRebindingHandler;
            GameEvents.AbilityRunnerEvents.OnLocalInputStartAbility += LocalInputStartAbilityHandler;
            GameEvents.AbilityRunnerEvents.OnLocalInputCancelAbility += LocalInputCancelAbilityHandler;
        }
    }

    public override void OnDestroy()
    {
        base.OnDestroy();
        if (IsServer)
        {
            GameEvents.GameComponentEvents.OnGameComponentSelected -= GameComponentSelectedHandler;
            GameEvents.GameComponentEvents.OnGameComponentConnected -= GameComponentConnectedHandler;
            GameEvents.GameComponentEvents.OnGameComponentDisconnected -= GameComponentDisconnectedHandler;
        }
        if (IsOwner)
        {
            GameEvents.UIEvents.OnGameComponentAbilitySelected -= GameComponentAbilitySelectedHandler;
            GameEvents.UIEvents.OnGameComponentAbilitySelectedEnd -= GameComponentAbilitySelectedEndHandler;
            GameEvents.RebindEvents.OnStartRebinding -= StartRebindingHandler;
            GameEvents.RebindEvents.OnFinishRebinding -= FinishRebindingHandler;
            GameEvents.AbilityRunnerEvents.OnLocalInputStartAbility -= LocalInputStartAbilityHandler;
            GameEvents.AbilityRunnerEvents.OnLocalInputCancelAbility -= LocalInputCancelAbilityHandler;
        }
    }

    private void GameComponentSelectedHandler(GameComponent gameComponent, bool isSelected)
    {
        PlayGameComponentSelectedSound_ClientRpc(isSelected);
    }
    [ClientRpc]
    private void PlayGameComponentSelectedSound_ClientRpc(bool isSelected)
    {
        if (isSelected && GameComponentSelectedClip != null)
        {
            audioSource.PlayOneShotSoundManaged(GameComponentSelectedClip, GameComponentSelectedVolume);
        }
    }
    private void GameComponentConnectedHandler(GameComponent gameComponent, GameComponent parentComponent)
    {
        PlayGameComponentConnectedSound_ClientRpc();
    }
    [ClientRpc]
    private void PlayGameComponentConnectedSound_ClientRpc()
    {
        if (GameComponentConnectedClip != null)
        {
            audioSource.PlayOneShotSoundManaged(GameComponentConnectedClip, GameComponentConnectedVolume);
        }
    }
    private void GameComponentDisconnectedHandler(GameComponent gameComponent, GameComponent parentComponent)
    {
        PlayGameComponentDisconnectedSound_ClientRpc();
    }
    [ClientRpc]
    private void PlayGameComponentDisconnectedSound_ClientRpc()
    {
        if (GameComponentDisconnectedClip != null)
        {
            audioSource.PlayOneShotSoundManaged(GameComponentDisconnectedClip, GameComponentDisconnectedVolume);
        }
    }
    private void GameComponentAbilitySelectedHandler(GameComponent gameComponent)
    {
        if (AbilitySelectedClip != null)
        {
            audioSource.PlayOneShotSoundManaged(AbilitySelectedClip, AbilitySelectedVolume);
        }
    }
    private void GameComponentAbilitySelectedEndHandler(GameComponent gameComponent)
    {
        if (AbilitySelectedEndClip != null)
        {
            audioSource.PlayOneShotSoundManaged(AbilitySelectedEndClip, AbilitySelectedEndVolume);
        }
    }
    private void StartRebindingHandler(int entryID)
    {
        if (StartRebindClip != null)
        {
            audioSource.PlayOneShotSoundManaged(StartRebindClip, StartRebindVolume);
        }
    }
    private void FinishRebindingHandler(int entryID, string path)
    {
        if (EndRebindClip != null)
        {
            audioSource.PlayOneShotSoundManaged(EndRebindClip, EndRebindVolume);
        }
    }
    private void LocalInputStartAbilityHandler(int abilityNumber)
    {
        if (StartAbilityClip != null)
        {
            audioSource.PlayOneShotSoundManaged(StartAbilityClip, StartAbilityVolume);
        }
    }
    private void LocalInputCancelAbilityHandler(int abilityNumber)
    {
        if (CancelAbilityClip != null)
        {
            audioSource.PlayOneShotSoundManaged(CancelAbilityClip, CancelAbilityVolume);
        }
    }
}