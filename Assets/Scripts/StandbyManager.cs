using UnityEngine;
using UnityEngine.SceneManagement;

internal class StandbyManager : MonoBehaviour
{
    [SerializeField]
    private GameObject _playerCountSelection;

    private string _lastSelectedScene;

    // Дефолтные настройки
    private void Start()
    {
        GameDataSingleton.Default();
        QualitySettings.SetQualityLevel(SettingsSingleton.QualityLevel, true);
        _lastSelectedScene = SceneManager.GetActiveScene().name;
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
        _playerCountSelection.SetActive(true);
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
        GameDataSingleton.PlayerCount = numberOfPlayers;
        SceneManager.LoadScene(_lastSelectedScene, LoadSceneMode.Single);
    }
}
