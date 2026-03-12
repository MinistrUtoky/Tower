using UnityEngine;

namespace Tower
{
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

        [Header("Transition")]
        public const string ENDGAME_MENU_SCENE = "EndgameMenu";
        public static Quaternion ShakeAngleFromTime(int totalFloors, float blockHeight, float shakeStartTime)
        {
            float globalScreenWidth = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, 0, 0)).x - Camera.main.ScreenToWorldPoint(Vector3.zero).x;
            float absoluteMaxTowerHeight = Mathf.Max(totalFloors, MAX_SHAKE_FLOORS) * blockHeight;
            float absoluteMaxAngleTan = (globalScreenWidth / 20f) / absoluteMaxTowerHeight;
            int floorHeight = Mathf.Min(MAX_SHAKE_FLOORS, totalFloors);
            float rotationAngleLimit = Mathf.Atan(absoluteMaxAngleTan) * Mathf.Rad2Deg * ((float)floorHeight) / ((float)MAX_SHAKE_FLOORS);
            float currentShakeAngle = Mathf.Sin((Time.time - shakeStartTime) * SHAKE_SPEED) * rotationAngleLimit; 
            return Quaternion.Euler(new Vector3(0, 0, currentShakeAngle));
        }
    }
}