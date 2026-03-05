using TMPro;
using UnityEngine;
using UnityEngine.UI;

internal class SettingsManager : AbstractPanelManager
{
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
        _nameField.text = SettingsSaveable.Instance.PlayerName;
        _music.value = SettingsSaveable.Instance.MusicLevel;
        _sounds.value = SettingsSaveable.Instance.SFXLevel;
       _qualityIndicator.text = SettingsSaveable.Instance.QualityLevelName;
    }

    public void OnNameChanged(string name)
    {
        Debug.Log("Name changed");
        SettingsSaveable.Instance.ChangeName(name);
        _nameField.text = SettingsSaveable.Instance.PlayerName;
    }

    public void ChangeQuality(bool toBetter)
    {
        AudioSingleton.Instance.PlaySfx(0, 0.5f);
        int currentQuality = SettingsSaveable.Instance.QualityLevel;
        SettingsSaveable.Instance.SetQualityPreset(toBetter ? currentQuality + 1 : currentQuality - 1);
        _qualityIndicator.text = SettingsSaveable.Instance.QualityLevelName;
    }

    public void OnSFXSlider(float value) => SettingsSaveable.Instance.SetSoundLevel(value);
    public void OnMusicSlider(float value) => SettingsSaveable.Instance.SetMusicLevel(value);

}
