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

    public int Score => GetInt(MConfig.SCORE);
    public bool[] AvailableLevels
    {
        get
        {
            string levelsJson = Get(MConfig.AVAIABLE_LEVELS);
            Debug.Log("Tried to get levels, got " +  levelsJson);
            if (levelsJson == "")
            {
                SaveOpenLevels(new bool[2] { false, false });
                levelsJson = Get(MConfig.AVAIABLE_LEVELS);
            }
            return JsonUtility.FromJson<LevelsAvailability>(levelsJson).Levels; 
        }
    }
    public void IncreaseScoreBy(int increase) => Save(MConfig.SCORE, (Score + increase).ToString());  
    public bool TryDecreaseScoreBy(int decrease)
    {
        if (Score < decrease) return false;
        Save(MConfig.SCORE, (Score - decrease).ToString());
        return true;
    }    

    public void SaveOpenLevels(bool[] levels) => 
        Save(MConfig.AVAIABLE_LEVELS, JsonUtility.ToJson(new LevelsAvailability(levels)));

    public void ResetOpenLevels() =>
        Save(MConfig.AVAIABLE_LEVELS, JsonUtility.ToJson(new LevelsAvailability(new bool[AvailableLevels.Length])));
}
