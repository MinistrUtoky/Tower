using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

internal class TransitionManager : MonoBehaviour
{
    [SerializeField]
    private GameObject[] _startGameSlides;
    [SerializeField]
    private int _playersLeft = 1;
    [SerializeField]
    private string _menuSceneName = "EndgameMenu";
    public static bool GameOn { get; private set; } = false;

    private void Awake()
    {
        foreach (var slide in _startGameSlides) slide.SetActive(false);
        GameOn = false;
        StopAllCoroutines();
        StartCoroutine(Startgame());
    }

    private IEnumerator Startgame()
    {
        yield return StartCoroutine(ShowSlides(_startGameSlides, 1f));
        GameOn = true;
    }

    public void KillPlayer(GameObject playerEndgameBanner, float reactionDelay)
    {
        _playersLeft -= 1;
        if (_playersLeft < 1)
        {
            GameOn = false;
            StopAllCoroutines();
            StartCoroutine(Endgame(reactionDelay));
        }
        else if (playerEndgameBanner != null)
        {
            StartCoroutine(KillAfter(playerEndgameBanner, reactionDelay));
        }
    }

    private IEnumerator KillAfter(GameObject playerEndgameBanner, float delay)
    {
        yield return new WaitForSeconds(delay);
        AudioSingleton.Instance.PlaySfx(3, 0.5f);
        yield return new WaitForSeconds(1f);
        playerEndgameBanner.SetActive(true);
    }

    private IEnumerator Endgame(float delay)
    {
        yield return new WaitForSeconds(delay);
        AudioSingleton.Instance.PlaySfx(3, 0.5f);
        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene(_menuSceneName);
    }

    private static IEnumerator ShowSlides(GameObject[] slides, float delay)
    {
        foreach (GameObject _slide in slides)
        {
            _slide.SetActive(true);
            yield return new WaitForSeconds(0.5f);
            float timer = delay * 5f;
            while (true)
            {
                timer -= Time.deltaTime;
                if (Input.anyKey || timer < 0f) 
                    break;
                yield return null;
            }
            _slide.SetActive(false);
        }
    }
}
