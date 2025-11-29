using System;
using System.Security.Cryptography;
using UnityEngine;

internal static class SettingsSingleton
{
    private const string Name = "PlayerName";
    private const string Music = "Music";
    private const string SoundFX = "SoundFX";
    private const string Quality = "QualityPreset";

    private static string RandomHash
    {
        get
        {
            byte[] bytes = new byte[64];
            using (var rng = new RNGCryptoServiceProvider())
            {
                rng.GetBytes(bytes);
            }
            return BitConverter.ToString(bytes);
        }
    }

    public static string PlayerName => PlayerPrefs.GetString(Name, "Player" + RandomHash);
    public static float MusicLevel => PlayerPrefs.GetFloat(Music, 1f);
    public static float SFXLevel => PlayerPrefs.GetFloat(SoundFX, 1f);
    public static int QualityLevel => PlayerPrefs.GetInt(Quality, 0);
    public static string QualityLevelName
    {
        get
        {
            int preset = QualityLevel;
            if (QualitySettings.names.Length - 1 < preset)
                preset = 0;
            else if (preset < 0)
                preset = QualitySettings.names.Length - 1;
            return QualitySettings.names[preset];
        }
    }

    private static bool _musicLevelChanged = false;
    public static bool MusicLevelChanged { 
        get 
        {
            if (!_musicLevelChanged) return false;
            bool changed = _musicLevelChanged;
            _musicLevelChanged = false;
            return changed;  
        }
    }

    public static void SetMusicLevel(float newLevel)
    {
        if (newLevel < 0f) newLevel = 0f;
        if (newLevel > 1f) newLevel = 1f;
        PlayerPrefs.SetFloat(Music, newLevel);
        _musicLevelChanged = true;
        PlayerPrefs.Save();
    } 

    public static void SetSoundLevel(float newLevel)
    {
        if (newLevel < 0f) newLevel = 0f;
        if (newLevel > 1f) newLevel = 1f;
        PlayerPrefs.SetFloat(SoundFX, newLevel);
        PlayerPrefs.Save();
    }

    public static void SetQualityPreset(int preset)
    {
        if (QualitySettings.names.Length - 1 < preset)
            preset = 0;
        else if (preset < 0)
            preset = QualitySettings.names.Length - 1;
        PlayerPrefs.SetInt(Quality, preset);
        PlayerPrefs.Save();
    } 
    
    public static void ChangeName(string newName)
    {
        PlayerPrefs.SetString(Name, newName);
    }
}
