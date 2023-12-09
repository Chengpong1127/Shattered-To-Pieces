using DigitalRuby.SoundManagerNamespace;
using UnityEngine;

public class SettingUpdater : MonoBehaviour
{
    void Awake()
    {
        GameEvents.OnSettingsUpdated += OnSettingsUpdatedHandler;
        GameSetting setting = ResourceManager.Instance.LoadLocalGameSetting();
        OnSettingsUpdatedHandler(setting);
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