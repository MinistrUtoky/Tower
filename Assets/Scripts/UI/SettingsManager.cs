using TMPro;
using UnityEngine;
using UnityEngine.UI;

internal class SettingsManager : MonoBehaviour
{
    [SerializeField]
    private GameObject _settingsPanel;
    [SerializeField]
    private TMP_InputField _nameField;
    [SerializeField]
    private TMP_Text _qualityIndicator;
    [SerializeField]
    private Slider _music;
    [SerializeField]
    private Slider _sounds;

    private void Start()
    {
        _nameField.text = SettingsSingleton.PlayerName;
        _music.value = SettingsSingleton.MusicLevel;
        _sounds.value = SettingsSingleton.SFXLevel;
       _qualityIndicator.text = SettingsSingleton.QualityLevelName;
    }

    public void OnSettingsButtonClicked()
    {
        AudioSingleton.Instance.PlaySfx(0, 0.5f);
        _settingsPanel.SetActive(true);
    }

    public void OnNameChanged(string name)
    {
        Debug.Log("Name changed");
        SettingsSingleton.ChangeName(name);
        _nameField.text = SettingsSingleton.PlayerName;
    }

    public void ChangeQuality(bool toBetter)
    {
        AudioSingleton.Instance.PlaySfx(0, 0.5f);
        int currentQuality = SettingsSingleton.QualityLevel;
        SettingsSingleton.SetQualityPreset(toBetter ? currentQuality + 1 : currentQuality - 1);
        _qualityIndicator.text = SettingsSingleton.QualityLevelName;
    }

    public void OnSFXSlider(float value) => SettingsSingleton.SetSoundLevel(value);
    public void OnMusicSlider(float value) => SettingsSingleton.SetMusicLevel(value);

    public void CloseSettings()
    {
        AudioSingleton.Instance.PlaySfx(0, 0.5f);
        _settingsPanel.SetActive(false);
    }
}
