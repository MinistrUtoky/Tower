using UnityEngine;

[RequireComponent(typeof(AudioSource))]
internal class AudioSingleton : MonoBehaviour
{
    public static AudioSingleton Instance { get; private set; }

    [SerializeField]
    private AudioClip[] availableSfx;

    [Header("SFX")]
    [SerializeField]
    private AudioSource _oneshotAso;
    [SerializeField]
    private AudioSource _loopedAso;

    [Header("Music")]
    [SerializeField]
    private AudioSource _musicAso;
    [SerializeField]
    private AudioClip _backgroundMusic;

    private void Awake()
    {
        if (FindObjectsByType<AudioSingleton>(FindObjectsSortMode.None).Length >= 2)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Update()
    {
        if (!_musicAso.isPlaying || SettingsSaveable.Instance.MusicLevelChanged)
            PlayMusic(_backgroundMusic);
    }

    private void PlayMusic(AudioClip _clip)
    {
        _musicAso.clip = _clip;
        _musicAso.volume = SettingsSaveable.Instance.MusicLevel;
        _musicAso.Play();
    }

    public void PlaySfx(int sfxID = -1, float vol = 1f)
    {
        if (sfxID == -1)
            sfxID = Random.Range(0, availableSfx.Length);
        _oneshotAso.PlayOneShot(availableSfx[sfxID], vol * SettingsSaveable.Instance.SFXLevel);
    }

    public void PlaySfxLooped(int sfxID, float volume)
    {
        _loopedAso.clip = availableSfx[sfxID];
        _loopedAso.loop = true;
        _loopedAso.volume = volume * SettingsSaveable.Instance.SFXLevel;

        if (!_loopedAso.isPlaying)
            _loopedAso.Play();
    }
    public void ChangeLoopedPitch(float newPitch) => _loopedAso.pitch = newPitch;
    public void StopSFX() { _oneshotAso.Stop(); _loopedAso.Stop(); }
}