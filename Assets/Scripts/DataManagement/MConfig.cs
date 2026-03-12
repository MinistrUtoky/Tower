using UnityEngine;

internal static class MConfig
{
    [Header("Point System")] 
    public const int PAYWALL_AMOUNT = 10;

    [Header("Arsenal Management")]
    public const string ARSENAL_SAVE_FILE_NAME = "YourArsenal.json";

    [Header("Congrats Management")]
    public const int SCORE_FONT_SIZE = 260;

    [Header("Progress Saving")]
    public const string SCORE = "Score";
    public const string AVAIABLE_LEVELS = "Levels";

    [Header("Settings Saving")]
    public const string NAME = "PlayerName";
    public const string MUSIC = "Music";
    public const string SOUNDFX = "SoundFX";
    public const string QUALITY = "QualityPreset";
}
