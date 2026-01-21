using System;
using UnityEngine;

internal static class ProgressSingleton
{
    [Serializable]
    public class LevelsAvailability
    {
        [SerializeField]
        private bool[] _levels = new bool[2];
        public bool[] Levels => _levels;
        public LevelsAvailability(bool[] levels) { _levels = levels; }
    }

    private const string SCORE = "Score";
    private const string AVAIABLE_LEVELS = "Levels";

    public static int Score => PlayerPrefs.GetInt(SCORE, 0);
    public static bool[] AvailableLevels
    {
        get
        {
            string levelsJson = PlayerPrefs.GetString(AVAIABLE_LEVELS, "{}");
            if (levelsJson == "{}")
            {
                SaveOpenLevels(new bool[2] { false, false });
                levelsJson = PlayerPrefs.GetString(AVAIABLE_LEVELS, "{}");
            }
            return JsonUtility.FromJson<LevelsAvailability>(levelsJson).Levels; 
        }
    }

    private static void SetScore(int newScore)
    {
        Debug.Log("Total score is set to " + newScore);
        PlayerPrefs.SetInt(SCORE, newScore);
        PlayerPrefs.Save();
    }

    public static void IncreaseScoreBy(int increase) => SetScore(Score + increase);  
    public static bool TryDecreaseScoreBy(int decrease)
    {
        if (Score < decrease) return false;
        SetScore(Score - decrease);
        return true;
    }    

    public static void SaveOpenLevels(bool[] levels)
    {
        PlayerPrefs.SetString(AVAIABLE_LEVELS, 
                              JsonUtility.ToJson(
                                  new LevelsAvailability(levels)));
        PlayerPrefs.Save();
    }

    public static void ResetOpenLevels()
    {
        PlayerPrefs.SetString(AVAIABLE_LEVELS,
                              JsonUtility.ToJson(
                                  new LevelsAvailability(new bool[AvailableLevels.Length])));
        PlayerPrefs.Save();
    }
}
