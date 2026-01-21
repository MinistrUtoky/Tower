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
            ShowFinale(InterplayData.PlayerCount == 2? _twoPlayerSetup : _singlePlayerSetup));
    }

    private IEnumerator ShowFinale(EndgameSetup setup)
    {
        if (setup.twoPlayer)
            setup.winnerCongratsHolder.text = InterplayData.Player1Score >= InterplayData.Player2Score ?
                                                                "Ура!\nПобедил Игрок 1" : "Ура!\nПобедил Игрок 2";

        ProgressSingleton.IncreaseScoreBy(InterplayData.Player1Score + InterplayData.Player2Score);
        print("New Score: " + ProgressSingleton.Score);

        AssembleResultScreen(setup.player1Info, setup.twoPlayer ? setup.player2Info : null, InterplayData.Player1Score, InterplayData.Player2Score);
        setup.showScreen.SetActive(true);

        yield return new WaitForSeconds(1f);
        while (true)
        {
            if (Input.touchCount > 0)
                break;
            yield return null;
        }
        print("Congratulation finished naturally");
        InterplayData.Default();
        SceneManager.LoadScene(0);
    }

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
