using Arsenal;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

internal class StandbyManager : AbstractPanelManager
{
    [Serializable]
    private struct LocationInfo {
        [SerializeField] private Button _locationButton;
        [SerializeField] private Button _locationBlocker;
        [SerializeField] private LocationPresetScriptable _locationPreset;
        public Button LocationButton => _locationButton;
        public Button LocationBlocker => _locationBlocker; 
        public LocationPresetScriptable LocationPreset => _locationPreset;
    }

    [SerializeField]
    private GameObject _locationSelection;
    [SerializeField]
    private LocationInfo[] _locations;
    [SerializeField]
    private GameObject _playerCountSelection;
    [SerializeField]
    private TMP_Text _debitBalance;

    private string _lastSelectedScene;
       
    private void Awake()
    {
        InterplayData.Default();
        QualitySettings.SetQualityLevel(SettingsSaveable.Instance.QualityLevel, true);
        _lastSelectedScene = SceneManager.GetActiveScene().name;
        for (int i = 0; i < _locations.Length; i++)
        {
            int index = i;
            _locations[i].LocationButton.onClick.AddListener(() => PrepareLocation(_locations[index].LocationPreset));
            if (_locations[i].LocationBlocker != null)
                _locations[i].LocationBlocker.onClick.AddListener(() => TryUnlock(index));
        }
        _debitBalance.text = "Score: " + ProgressSaveable.Instance.Score.ToString();
        RefreshButtonsAvailability();
    }
    public void ChangeSceneTo(string sceneName) => _lastSelectedScene = sceneName;  
    public void OnMainMenuButtonClicked() => SetActive(_locationSelection, true);
    public void StartSelectedWith(int numberOfPlayers)
    {
        AudioSingleton.Instance.PlaySfx(0, 0.5f);
        QualitySettings.SetQualityLevel(SettingsSaveable.Instance.QualityLevel, true);
        InterplayData.PlayerCount = numberOfPlayers;
        SceneManager.LoadScene(_lastSelectedScene, LoadSceneMode.Single);
    }
    private void PrepareLocation(LocationPresetScriptable preset)
    {
        preset.BuildAlias();
        InterplayData.NextPlayPreset(preset, ArsenalManager.ArsenalPreset);
        SetActive(_playerCountSelection, true);
        _locationSelection.SetActive(false);
    }
    private void TryUnlock(int index)
    {
        Debug.Log("Passed on index " + index + " for an unlock");
        bool[] prev = ProgressSaveable.Instance.AvailableLevels;
        for (int i=0;i<prev.Length;i++)
            Debug.Log("Availability of button " + i + " is " + prev[i]);

        if (index > prev.Length - 1) Debug.LogError("Index for an unlockable levels is out of bounds");

        if (ProgressSaveable.Instance.TryDecreaseScoreBy(MConfig.PAYWALL_AMOUNT))
        {
            gameObject.SetActive(false);
            if (prev.Length != _locations.Length)
            {
                bool[] temp = new bool[_locations.Length];
                for (int i = 0; i < Mathf.Min(temp.Length, prev.Length); i++) 
                    temp[i] = prev[i];
                prev = temp;
            }
            prev[index] = true;
            ProgressSaveable.Instance.SaveOpenLevels(prev);
            _debitBalance.text = "Score: " + ProgressSaveable.Instance.Score.ToString();
            RefreshButtonsAvailability();
        }
    }

    private void RefreshButtonsAvailability()
    {
        bool[] availability = ProgressSaveable.Instance.AvailableLevels;
        for (int i = 1; i < _locations.Length; i++)
            if (i < availability.Length)
                _locations[i].LocationBlocker.gameObject.SetActive(!availability[i]);
            else 
                break;
    }
}
