using UnityEngine;

public static class InterplayData
{
    private static int _playerCount = 1;
    private static int _player1Score = 0;
    private static int _player2Score = 0;

    private static LocationPresetScriptable _selectedPreset = new();

    public static int PlayerCount
    {
        get => _playerCount;
        set
        {
            if (value == 1 || value == 2) _playerCount = value;
            else Debug.LogError("Can't set player count to anything other than 1 or 2");
        }
    }

    public static int Player1Score
    {
        get => _player1Score;
        set
        {
            if (value > -1) _player1Score = value;
            else Debug.LogError("Player 1 score can't be negative");
        }
    }
    public static int Player2Score
    {
        get => _player2Score;
        set
        {
            if (value > -1) _player2Score = value;
            else Debug.LogError("Player 2 score can't be negative");
        }
    }

    internal static LocationPresetScriptable Location 
    {
        get => _selectedPreset;
        set { _selectedPreset = value; }
    }

    public static void Default()
    {
        _player1Score = 0;
        _player2Score = 0;
        _playerCount = 1;
        _selectedPreset = new();
    }
}
