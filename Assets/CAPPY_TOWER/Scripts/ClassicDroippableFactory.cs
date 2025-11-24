using DG.Tweening;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

internal class ClassicDroppableFactory : AbstractDroppableFactory
{
    private enum ScreenRelation
    {
        Full,
        LeftHalf,
        RightHalf
    }

    [Header("Gameplay")]
    [SerializeField]
    private ScreenRelation _clickScreenPart = ScreenRelation.Full;
    [SerializeField]
    private GameObject[] _droppables;
    [SerializeField]
    private GameObject[] _rareDroppables;
    [SerializeField]
    private Transform _towerRoot;
    [SerializeField]
    private PendulumPhysical _pendulum;
    [SerializeField]
    private Camera _cameraToSpawn;
    [SerializeField]
    private Canvas _background;

    [Header("UI")]
    [SerializeField]
    private TMP_Text _score;
    [SerializeField]
    private Image _heartsBG;
    [SerializeField]
    private Image[] _hearts;

    [Header("Endgame")]
    [SerializeField]
    private TransitionManager _endgameManager;
    [SerializeField]
    private GameObject _banner;
    [SerializeField]
    private TMP_Text _endgameResult;

    // Время начала покачивания башни
    private float _shakeStartTime;
    private bool _isShaking = false;

    private int _hp = 3;
    private bool _isHanging = false;

    private float _currentScreenWidthCoef = 1f;
    private float _baseCameraOrthoWidth;

    private float _startGameTouchBlock = TConfig.LOADING_TIME;


    private void Awake()
    {
        _baseCameraOrthoWidth = _cameraToSpawn.orthographicSize;
    }

    protected new void Start()
    {
        base.Start();
        Assert.IsNotNull(_endgameManager);
        Assert.IsNotNull(_towerRoot);
        Assert.IsNotNull(_pendulum);
        SpawnRandomDroppable();
        _startGameTouchBlock = TConfig.LOADING_TIME;
    }

    private void Update()
    {
        MoveCurrent();
        if (!TransitionManager.GameOn) return;
        // Если игра началась разблокируем нажатие экрана по истечению загрузки
        if (_startGameTouchBlock > 0f)
        {
            _startGameTouchBlock -= Time.deltaTime;
            return;
        }

        if (IsAlive & _isHanging & Input.touchCount > 0)
        {
            if (_clickScreenPart == ScreenRelation.Full
                || _clickScreenPart == ScreenRelation.LeftHalf & Input.touches[0].position.y > Screen.height / 2
                || _clickScreenPart == ScreenRelation.RightHalf & Input.touches[0].position.y <= Screen.height / 2)
                DropCurrent();
        }
        ShakeTower();
    }

    public override void Add(IDroppable towerBlock)
    {
        base.Add(towerBlock);
        _currentScreenWidthCoef += TConfig.SCREEN_WIDTH_INCREMENT;
        if (_currentScreenWidthCoef > TConfig.MAX_SCREEN_WIDTH_COEF)
            _currentScreenWidthCoef = TConfig.MAX_SCREEN_WIDTH_COEF;

        // заменил bounds.extents на size
        // заменил магическую константу определяющим ее размером объекта
        float newHeight = -towerBlock.Collider.size.y * towerBlock.Collider.transform.localScale.y * (TotalFloors > 1 ? TotalFloors - 2 : 0);
        // Фон перестает двигаться, как только башня достигает его вершины
        if (TotalFloors > TConfig.BACKGROUND_BLOCK_HEIGHT_LIMIT)
        {
            float newBackgroundHeight = towerBlock.Collider.size.y
                                            * towerBlock.Collider.transform.localScale.y
                                                * (TotalFloors - TConfig.BACKGROUND_BLOCK_HEIGHT_LIMIT);
            _background.transform.parent.DOKill();
            _background.transform.DOLocalMoveY(newBackgroundHeight, 1f); // движение должно быть локальным
        }
        _towerRoot.transform.parent.DOKill();
        _towerRoot.transform.parent.DOLocalMoveY(newHeight, 1f); // теперь локальное
        _score.text = TotalFloors.ToString();
        _pendulum.SpeedUpPendulum();
        _cameraToSpawn.DOKill();
        _cameraToSpawn.DOOrthoSize(_currentScreenWidthCoef * _baseCameraOrthoWidth, 1f);

    }
    public override void TakeHit()
    {
        _hp -= 1;
        Destroy(_hearts[_hp]);
        _heartsBG.DOKill();
        _heartsBG.DOFade(0, 0.2f)
                    .SetEase(Ease.Linear)
                    .SetLoops(4, LoopType.Yoyo);
        if (_hp < 1)
        {
            Die();
            return;
        }

        for (int i = 0; i < _hp; i++)
        {
            _hearts[i].DOKill();
            _hearts[i].DOFade(0, 0.2f)
                        .SetEase(Ease.Linear)
                        .SetLoops(4, LoopType.Yoyo);
        }
        SpawnRandomDroppable();
    }

    public override void Die()
    {
        base.Die();
        if (_clickScreenPart == ScreenRelation.RightHalf)
            GameDataSingleton.Player2Score = TotalFloors;
        else
            GameDataSingleton.Player1Score = TotalFloors;
        if (_endgameResult != null)
            _endgameResult.text = TotalFloors.ToString();
        _endgameManager.KillPlayer(_banner, 1f);
    }

    public override void SpawnRandomDroppable()
    {
        GameObject toSpawn;
        if (Random.Range(0, 1f) > 0.95f)
            toSpawn = _rareDroppables[Random.Range(0, _rareDroppables.Length)];
        else
            toSpawn = _droppables[Random.Range(0, _droppables.Length)];
        IDroppable droppable = Instantiate(toSpawn,
                                        new Vector3(_pendulum.transform.position.x,
                                                    _pendulum.transform.position.y),
                                        Quaternion.identity, _towerRoot)
                                        .transform.GetChild(0).GetComponent<AbstractDroppableBlock>();
        base.SpawnDroppable(droppable);
        _isHanging = true;
    }

    protected override void DropCurrent()
    {
        _isHanging = false;
        base.DropCurrent();
    }

    private void MoveCurrent()
    {
        if (Current != null)
            Current.Collider.transform.position = new Vector3(_pendulum.transform.position.x,
                                                        _pendulum.transform.position.y);
    }

    /// <summary>
    /// Гармонические колебания башни, увеличивающиеся с ростом башни
    /// </summary>
    private void ShakeTower()
    {
        if (TotalFloors > TConfig.MIN_SHAKE_FLOORS)
        {
            if (!_isShaking)
            {
                _shakeStartTime = Time.time;
                _isShaking = true;
            }
            int floorHeight = Mathf.Min(TConfig.MAX_SHAKE_FLOORS, TotalFloors);
            float globalScreenWidth = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, 0, 0)).x - Camera.main.ScreenToWorldPoint(Vector3.zero).x;
            float abstractHeight = floorHeight * Current.Collider.size.y;
            float absoluteMaxTowerHeight = Mathf.Max(TotalFloors, TConfig.MAX_SHAKE_FLOORS) * Current.Collider.size.y;
            float absoluteMaxAngleTan = (globalScreenWidth / 20f) / absoluteMaxTowerHeight;
            float rotationAngleLimit = Mathf.Atan(absoluteMaxAngleTan) * Mathf.Rad2Deg * ((float)floorHeight) / ((float)TConfig.MAX_SHAKE_FLOORS);
            float currentShakeAngle = Mathf.Sin((Time.time - _shakeStartTime) * TConfig.SHAKE_SPEED) * rotationAngleLimit;
            _towerRoot.rotation = Quaternion.Euler(new Vector3(0, 0, currentShakeAngle));
        }
    }
}
