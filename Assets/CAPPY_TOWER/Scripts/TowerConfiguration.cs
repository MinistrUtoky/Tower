using UnityEngine;

internal static class TConfig
{
    [Header("Main parameters")]
    public const int MAX_HEIGHT = 12;

    [Header("Shake parameters")]
    // Высота, на которой начинаются покачивания Башни
    public const int MIN_SHAKE_FLOORS = 5;
    // Высота, на которой покачивания достигают максимальной амплитуды
    public const int MAX_SHAKE_FLOORS = 30;
    // Коэффициент скорость покачивания Башни
    public const float SHAKE_SPEED = 0.66f;

    [Header("Increasing screen width")]
    public const float MAX_SCREEN_WIDTH_COEF = 1.84f;
    public const float SCREEN_WIDTH_INCREMENT = 0.03f;

    [Header("Background height")]
    // Ограничение по движению фона в блоках
    public const int BACKGROUND_BLOCK_HEIGHT_LIMIT = 100; // 40 в идеале

    [Header("Loading")]
    // Время загрузочного экрана
    public const float LOADING_TIME = 0.5f;
}
