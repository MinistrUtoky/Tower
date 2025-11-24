using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

[Serializable]
internal class PlayerEndgameInfo
{
    public GameObject loserBackground;
    public GameObject winnerBackground;
    public TMP_Text resultHolder;
}
[Serializable]
internal class EndgameSetup
{
    public bool twoPlayer;
    public GameObject showScreen;

    public PlayerEndgameInfo player1Info;
    public PlayerEndgameInfo player2Info;

    public TMP_Text winnerCongratsHolder;
}

internal class CongratsManager : MonoBehaviour
{

    [SerializeField]
    private EndgameSetup _singlePlayerSetup;
    [SerializeField]
    private EndgameSetup _twoPlayerSetup;

    private const int SCORE_FONT_SIZE = 260;

    private void Awake()
    {
        if (AudioSingleton.Instance)
            AudioSingleton.Instance.StopSFX();
        StartCoroutine(
            ShowFinale(GameDataSingleton.PlayerCount == 2? _twoPlayerSetup : _singlePlayerSetup));
    }

    /// <summary>
    /// Экран завершения игры
    /// </summary>
    private IEnumerator ShowFinale(EndgameSetup setup)
    {
        if (setup.twoPlayer)
            setup.winnerCongratsHolder.text = GameDataSingleton.Player1Score >= GameDataSingleton.Player2Score ?
                                                                "Ура!\nПобедил Игрок 1" : "Ура!\nПобедил Игрок 2";
        // Собирается информация в зависимости от результата
        AssembleResultScreen(setup.player1Info, setup.twoPlayer ? setup.player2Info : null, GameDataSingleton.Player1Score, GameDataSingleton.Player2Score);
        setup.showScreen.SetActive(true);

        // Ждем действия игрока и выключаем
        yield return new WaitForSeconds(1f);
        while (true)
        {
            if (Input.touchCount > 0)
                break;
            yield return null;
        }
        print("Congratulation finished naturally");
        GameDataSingleton.Default();
        SceneManager.LoadScene(0);
    }

    /// <summary>
    /// Автоматически меняет размер очков результата и фон очков
    /// в соответствии с полученным числом очков и в зависимости от того,
    /// кто из игроков набрал больше
    /// </summary>
    private static void AssembleResultScreen(PlayerEndgameInfo player1Card, PlayerEndgameInfo player2Card, int points1, int points2)
    {
        player1Card.resultHolder.text = points1.ToString();
        player1Card.resultHolder.fontSize = Mathf.Min(SCORE_FONT_SIZE * 2f / player1Card.resultHolder.text.Length, SCORE_FONT_SIZE);
        if (player2Card != null)
        {
            player1Card.resultHolder.transform.localPosition = new Vector3(player1Card.resultHolder.transform.localPosition.x,
                                                                            Mathf.Max(10f * (player1Card.resultHolder.text.Length - 2), 0f) - 20f,
                                                                            player1Card.resultHolder.transform.localPosition.z);
            player2Card.resultHolder.text = points2.ToString();
            player2Card.resultHolder.fontSize = Mathf.Min(SCORE_FONT_SIZE * 2f / player2Card.resultHolder.text.Length, SCORE_FONT_SIZE);
            player2Card.resultHolder.transform.localPosition = new Vector3(player1Card.resultHolder.transform.localPosition.x,
                                                                            Mathf.Max(10f * (player1Card.resultHolder.text.Length - 2), 0f) - 20f,
                                                                            player1Card.resultHolder.transform.localPosition.z);
        }
        else
            player1Card.resultHolder.transform.localPosition = new Vector3(player1Card.resultHolder.transform.localPosition.x,
                                                                            Mathf.Max(10f * (player1Card.resultHolder.text.Length - 2), 0f),
                                                                            player1Card.resultHolder.transform.localPosition.z);

        if (points1 >= points2)
        {
            player1Card.winnerBackground.SetActive(true);
            player2Card?.loserBackground.SetActive(true);
        }
        else
        {
            player1Card.loserBackground.SetActive(true);
            player2Card?.winnerBackground.SetActive(true);
        }
    }
}
