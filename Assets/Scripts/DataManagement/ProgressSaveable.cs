using System;
using UnityEngine;

internal class ProgressSaveable : AbstractSaveable<ProgressSaveable>
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
    public int Score => GetInt(SCORE);
    public bool[] AvailableLevels
    {
        get
        {
            string levelsJson = Get(AVAIABLE_LEVELS);
            if (levelsJson == "")
            {
                SaveOpenLevels(new bool[2] { false, false });
                levelsJson = Get(AVAIABLE_LEVELS);
            }
            return JsonUtility.FromJson<LevelsAvailability>(levelsJson).Levels; 
        }
    }
    public void IncreaseScoreBy(int increase) => Save(SCORE, (Score + increase).ToString());  
    public bool TryDecreaseScoreBy(int decrease)
    {
        if (Score < decrease) return false;
        Save(SCORE, (Score - decrease).ToString());
        return true;
    }    

    public void SaveOpenLevels(bool[] levels) => 
        Save(AVAIABLE_LEVELS, JsonUtility.ToJson(new LevelsAvailability(levels)));

    public void ResetOpenLevels() =>
        Save(AVAIABLE_LEVELS, JsonUtility.ToJson(new LevelsAvailability(new bool[AvailableLevels.Length])));
}
