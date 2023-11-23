using UnityEngine;
using UnityEngine.UI;

public class SettingWidgetController : GameWidgetController
{
    [SerializeField]
    private Slider _masterVolumeSlider;
    [SerializeField]
    private Slider _musicVolumeSlider;
    [SerializeField]
    private Slider _soundVolumeSlider;
    private GameSetting _gameSetting;
    void Awake()
    {
        Debug.Assert(_masterVolumeSlider != null);
        Debug.Assert(_musicVolumeSlider != null);
        Debug.Assert(_soundVolumeSlider != null);
        _masterVolumeSlider.onValueChanged.AddListener(OnMasterVolumeSliderValueChangedHandler);
        _musicVolumeSlider.onValueChanged.AddListener(OnMusicVolumeSliderValueChangedHandler);
        _soundVolumeSlider.onValueChanged.AddListener(OnSoundVolumeSliderValueChangedHandler);
    }
    public override void Show()
    {
        base.Show();
        _gameSetting = ResourceManager.Instance.LoadLocalGameSetting();
        SetSettingUI(_gameSetting);
    }

    private void SetSettingUI(GameSetting setting){
        _masterVolumeSlider.value = setting.MasterVolume;
        _musicVolumeSlider.value = setting.MusicVolume;
        _soundVolumeSlider.value = setting.SoundVolume;
    }
    private void OnMasterVolumeSliderValueChangedHandler(float value){
        _gameSetting.MasterVolume = value;
        ResourceManager.Instance.SaveLocalGameSetting(_gameSetting);
    }
    private void OnMusicVolumeSliderValueChangedHandler(float value){
        _gameSetting.MusicVolume = value;
        ResourceManager.Instance.SaveLocalGameSetting(_gameSetting);
    }
    private void OnSoundVolumeSliderValueChangedHandler(float value){
        _gameSetting.SoundVolume = value;
        ResourceManager.Instance.SaveLocalGameSetting(_gameSetting);
    }
}