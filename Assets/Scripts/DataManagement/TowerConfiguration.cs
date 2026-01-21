using UnityEngine;

internal static class TConfig
{
    [Header("Main parameters")]
    public const int MAX_HEIGHT = 12;

    [Header("Shake parameters")]
    public const int MIN_SHAKE_FLOORS = 5;
    public const int MAX_SHAKE_FLOORS = 30;
    public const float SHAKE_SPEED = 0.66f;

    [Header("Increasing screen width")]
    public const float MAX_SCREEN_WIDTH_COEF = 1.84f;
    public const float SCREEN_WIDTH_INCREMENT = 0.03f;

    [Header("Background height")]
    public const int BACKGROUND_BLOCK_HEIGHT_LIMIT = 100;

    [Header("Loading")]
    public const float LOADING_TIME = 0.5f;
}
