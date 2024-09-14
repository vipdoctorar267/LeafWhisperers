using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class AudioSettings
{
    public float FXGlobalVolume { get; set; }
    public float BGGlobalVolume { get; set; }
}

public class AudioSettingsManager : MonoBehaviour
{
    [SerializeField] private Slider BgVolumeSlider;
    [SerializeField] private Slider FxVolumeSlider;

    private DataManager _dataManager;
    private AudioSettings _audioSettings;

    private void Awake()
    {
        _dataManager = FindObjectOfType<DataManager>();
        LoadAudioSettings();
    }

    void OnEnable()
    {
        _dataManager = FindObjectOfType<DataManager>();
        LoadAudioSettings();
        if (FxVolumeSlider != null)
        {
            FxVolumeSlider.maxValue = 100;
            FxVolumeSlider.value = _audioSettings.FXGlobalVolume * 100;
            FxVolumeSlider.onValueChanged.AddListener(OnFXVolumeChanged);
        }

        if (BgVolumeSlider != null)
        {
            BgVolumeSlider.maxValue = 100;
            BgVolumeSlider.value = _audioSettings.BGGlobalVolume * 100;
            BgVolumeSlider.onValueChanged.AddListener(OnBGVolumeChanged);
        }
    }

    private void OnFXVolumeChanged(float value)
    {
        _audioSettings.FXGlobalVolume = value / 100f;
    }

    private void OnBGVolumeChanged(float value)
    {
        _audioSettings.BGGlobalVolume = value / 100f;
    }

    private void LoadAudioSettings()
    {
        _dataManager.LoadAudioSettings();
        _audioSettings = _dataManager._audioSetting;

        if (_audioSettings == null)
        {
            _audioSettings = new AudioSettings
            {
                FXGlobalVolume = 1f,
                BGGlobalVolume = 1f
            };
            SaveAudioSettings();
        }
    }

    private void SaveAudioSettings()
    {
        _dataManager._audioSetting = _audioSettings;
        _dataManager.SaveAudioSettings();
    }

    public void OnButtonClickSave()
    {
        SaveAudioSettings();
        InGameAudioManager.Instance.SetFXGlobalVolume(_audioSettings.FXGlobalVolume);
        BackGroundAudioManager.Instance.SetBgGlobalVolume(_audioSettings.BGGlobalVolume);
    }
}
