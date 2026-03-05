using System;
using System.Security.Cryptography;
using UnityEngine;

internal class SettingsSaveable : AbstractSaveable<SettingsSaveable>
{
    private const string NAME = "PlayerName";
    private const string MUSIC = "Music";
    private const string SOUNDFX = "SoundFX";
    private const string QUALITY = "QualityPreset";

    private static string RandomHash
    {
        get
        {
            byte[] bytes = new byte[64];
            using (var rng = new RNGCryptoServiceProvider()) { rng.GetBytes(bytes); }
            return BitConverter.ToString(bytes);
        }
    }

    public string PlayerName { 
        get { 
            string name = Get(NAME); 
            return name == ""? "Player" + RandomHash : name; 
        } 
    }
    public float MusicLevel => GetFloat(MUSIC);
    public float SFXLevel => GetFloat(SOUNDFX);
    public int QualityLevel => GetInt(QUALITY);
    public string QualityLevelName
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

    private bool _musicLevelChanged = false;
    public bool MusicLevelChanged { 
        get {   
            if (!_musicLevelChanged) return false;
            _musicLevelChanged = false;
            return true;  
        }
    }

    public void SetMusicLevel(float newLevel)
    {
        Save(MUSIC, Math.Min(1f, Math.Max(0f, newLevel)).ToString());
        _musicLevelChanged = true;
    } 
    public void SetSoundLevel(float newLevel) => Save(SOUNDFX, Math.Min(1f, Math.Max(0f, newLevel)).ToString());
    public void SetQualityPreset(int preset)
    {
        if (QualitySettings.names.Length - 1 < preset)
            preset = 0;
        else if (preset < 0)
            preset = QualitySettings.names.Length - 1;
        Save(QUALITY, preset.ToString());
    } 
    
    public void ChangeName(string newName) => Save(NAME, newName);
}
