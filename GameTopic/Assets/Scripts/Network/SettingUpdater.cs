using DigitalRuby.SoundManagerNamespace;
using UnityEngine;

public class SettingUpdater : MonoBehaviour
{
    void Awake()
    {
        GameEvents.OnSettingsUpdated += OnSettingsUpdatedHandler;
    }

    private void OnSettingsUpdatedHandler(GameSetting setting)
    {
        AudioListener.volume = setting.MasterVolume;
        SoundManager.SoundVolume = setting.SoundVolume;
        SoundManager.MusicVolume = setting.MusicVolume;
    }
    void OnDestroy()
    {
        GameEvents.OnSettingsUpdated -= OnSettingsUpdatedHandler;
    }
}