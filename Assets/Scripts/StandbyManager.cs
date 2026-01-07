using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

internal class StandbyManager : MonoBehaviour
{
    [SerializeField]
    private GameObject _locationSelection;
    [SerializeField]
    private Button[] _locationBlockers;
    [SerializeField]
    private GameObject _playerCountSelection;
    [SerializeField]
    private TMP_Text _debitBalance;

    private string _lastSelectedScene;

    private const int PAYWALL_AMOUNT = 10;

    // Дефолтные настройки
    private void Start()
    {
        InterplayData.Default();
        QualitySettings.SetQualityLevel(SettingsSingleton.QualityLevel, true);
        _lastSelectedScene = SceneManager.GetActiveScene().name;

        for (int i = 0; i < _locationBlockers.Length; i++)
        {
            int index = i;
            _locationBlockers[i].onClick.AddListener(() => TryUnlock(index));
        }

        _debitBalance.text = "Score: " + ProgressSingleton.Score.ToString();
        RefreshButtonsAvailability();
    }

    /// <summary>
    /// Так как кнопки в Unity воспринимают только события с 0-1 аргументами, 
    /// то нужно два разных вызова для выбора сцены и числа игроков
    /// </summary>
    public void ChangeSceneTo(string sceneName)
    {
        _lastSelectedScene = sceneName;
    }

    /// <summary>
    /// По нажатию начала игры мы открываем панель выбора числа игроков
    /// </summary>
    public void OnMainMenuButtonClicked()
    {
        AudioSingleton.Instance.PlaySfx(0, 0.5f);
        _locationSelection.SetActive(true);
    }
    /// <summary>
    /// По нажатию числа игроков мы записываем это число в синглтон передачи данных 
    /// (потому что PlayerPrefs имеет отвратительную систему индексации) 
    /// И начинаем игру с нужными настройками (для графики пока плейсхолдер, а звук сам себе синглтон)
    /// </summary>
    public void StartSelectedWith(int numberOfPlayers)
    {
        AudioSingleton.Instance.PlaySfx(0, 0.5f);
        QualitySettings.SetQualityLevel(SettingsSingleton.QualityLevel, true);
        InterplayData.PlayerCount = numberOfPlayers;
        SceneManager.LoadScene(_lastSelectedScene, LoadSceneMode.Single);
    }

    public void PrepareLocation(BlockPresetScriptable preset)
    {
        AudioSingleton.Instance.PlaySfx(0, 0.5f);
        InterplayData.Location = preset;
        _playerCountSelection.SetActive(true);
        _locationSelection.SetActive(false);
    }

    private void TryUnlock(int index)
    {
        Debug.Log("Passed on index " + index + " for an unlock");
        bool[] prev = ProgressSingleton.AvailableLevels;
        if (index > prev.Length - 1)
            Debug.LogError("Index for an unlockable levels is out of bounds");

        if (ProgressSingleton.TryDecreaseScoreBy(PAYWALL_AMOUNT))
        {
            gameObject.SetActive(false);
            if (prev.Length != _locationBlockers.Length)
            {
                bool[] temp = new bool[_locationBlockers.Length];
                for (int i = 0; i < Mathf.Min(temp.Length, prev.Length); i++) 
                    temp[i] = prev[i];
                prev = temp;
            }
            prev[index] = true;
            ProgressSingleton.SaveOpenLevels(prev);
            _debitBalance.text = "Score: " + ProgressSingleton.Score.ToString();
            RefreshButtonsAvailability();
        }
    }

    private void RefreshButtonsAvailability()
    {
        bool[] availability = ProgressSingleton.AvailableLevels;
        for (int i = 0; i < _locationBlockers.Length; i++)
            if (i < availability.Length)
                _locationBlockers[i].gameObject.SetActive(!availability[i]);
            else 
                break;
    }

}
