using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;


internal class ClassicDroppableFactory : AbstractDroppableFactory
{
    public enum ScreenRelation
    {
        Full,
        LeftHalf,
        RightHalf
    }
    [Header("Gameplay")]
    [SerializeField]
    private ScreenRelation _clickScreenPart = ScreenRelation.Full;
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
    [SerializeField]
    private Image _pendulumVisual;
    [SerializeField]
    private Image _pendulumHolderVisual;

    [Header("Endgame")]
    [SerializeField]
    private GameObject _banner;
    [SerializeField]
    private TMP_Text _endgameResult;

    private BlockPresetScriptable _blocksPreset;

    private float _shakeStartTime;
    private bool _isShaking = false;

    private int _hp = 3;

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
        Assert.IsNotNull(TransitionManager.Instance);
        Assert.IsNotNull(_towerRoot);
        Assert.IsNotNull(_pendulum);
        _blocksPreset = InterplayData.Location;
        _pendulumVisual.sprite = _blocksPreset.PendulumImage;
        _pendulumHolderVisual.sprite = _blocksPreset.PendulumHolderImage;
        SpawnRandomDroppable();
        _startGameTouchBlock = TConfig.LOADING_TIME;
    }

    private void Update()
    {
        MoveCurrent();
        if (!TransitionManager.Instance.GameOn) return;
        if (_startGameTouchBlock > 0f)
        {
            _startGameTouchBlock -= Time.deltaTime;
            return;
        }

        if (IsAlive & IsHanging & Input.touchCount > 0)
        {
            if (_clickScreenPart == ScreenRelation.Full
                || _clickScreenPart == ScreenRelation.LeftHalf & Input.touches[0].position.y > Screen.height / 2f
                || _clickScreenPart == ScreenRelation.RightHalf & Input.touches[0].position.y <= Screen.height / 2f)
                DropCurrent();
        }
        if (Current != null)
            ShakeTower();
    }

    public override void Add(IDroppable towerBlock)
    {
        base.Add(towerBlock);
        towerBlock.Collider.transform.parent.parent = _towerRoot;
        _currentScreenWidthCoef = Mathf.Min(TConfig.MAX_SCREEN_WIDTH_COEF, 
                                                _currentScreenWidthCoef + TConfig.SCREEN_WIDTH_INCREMENT);

        float newHeight = -towerBlock.Collider.size.y * towerBlock.Collider.transform.localScale.y * (TotalFloors > 1 ? TotalFloors - 2 : 0);

        if (TotalFloors > TConfig.BACKGROUND_BLOCK_HEIGHT_LIMIT)
        {
            float newBackgroundHeight = towerBlock.Collider.size.y
                                            * towerBlock.Collider.transform.localScale.y
                                                * (TotalFloors - TConfig.BACKGROUND_BLOCK_HEIGHT_LIMIT);
            _background.transform.parent.DOKill();
            _background.transform.DOLocalMoveY(newBackgroundHeight, 1f); 
        }
        _towerRoot.transform.parent.DOKill();
        _towerRoot.transform.parent.DOLocalMoveY(newHeight, 1f); 
        _score.text = TotalFloors.ToString();
        _pendulum.SpeedUpPendulum();
        _cameraToSpawn.DOKill();
        _cameraToSpawn.DOOrthoSize(_currentScreenWidthCoef * _baseCameraOrthoWidth, 1f);
    }

    public override void RemoveTopmost()
    {
        base.RemoveTopmost();
        _currentScreenWidthCoef = Mathf.Min(TConfig.MAX_SCREEN_WIDTH_COEF,
                                                _currentScreenWidthCoef + TConfig.SCREEN_WIDTH_INCREMENT);

        float newHeight = -Current.Collider.size.y * Current.Collider.transform.localScale.y * (TotalFloors > 1 ? TotalFloors - 2 : 0);

        if (TotalFloors > TConfig.BACKGROUND_BLOCK_HEIGHT_LIMIT)
        {
            float newBackgroundHeight = Current.Collider.size.y
                                            * Current.Collider.transform.localScale.y
                                                * (TotalFloors - TConfig.BACKGROUND_BLOCK_HEIGHT_LIMIT);
            _background.transform.parent.DOKill();
            _background.transform.DOLocalMoveY(newBackgroundHeight, 1f); 
        }
        _towerRoot.transform.parent.DOKill();
        _towerRoot.transform.parent.DOLocalMoveY(newHeight, 1f); 
        _score.text = TotalFloors.ToString();
        _pendulum.SpeedUpPendulum();
        _cameraToSpawn.DOKill();
        _cameraToSpawn.DOOrthoSize(_currentScreenWidthCoef * _baseCameraOrthoWidth, 1f);
    }

    public override void TakeHit()
    {
        if (_hp < 1) return;
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
    }

    public override void Die()
    {
        base.Die();
        if (_clickScreenPart == ScreenRelation.RightHalf)
            InterplayData.Player2Score = TotalFloors;
        else
            InterplayData.Player1Score = TotalFloors;
        if (_endgameResult != null)
            _endgameResult.text = TotalFloors.ToString();
        TransitionManager.Instance.KillPlayer(_banner, 1f);
    }

    public override void SpawnRandomDroppable()
    {
        Vector3 whereToSpawn = new Vector3(_pendulum.transform.position.x, _pendulum.transform.position.y);

        BlockPresetScriptable.Block block = _blocksPreset.RandomBlock();
        IDroppable droppable = Instantiate(block.Prefab, whereToSpawn, Quaternion.identity)
                                                    .transform.GetChild(0).GetComponent<IDroppable>();
        droppable.Image.sprite = block.Image;
        base.SpawnDroppable(droppable);
    }

    private void MoveCurrent()
    {
        if (IsHanging)
            Current.Collider.transform.position = new Vector3(_pendulum.transform.position.x,
                                                              _pendulum.transform.position.y);
    }

    private void ShakeTower()
    {       
        if (TotalFloors > TConfig.MIN_SHAKE_FLOORS)
        {
            if (!_isShaking)
            {
                _shakeStartTime = Time.time;
                _isShaking = true;
            }
            float currentShakeAngle = GetShakeAngleFromTime(TotalFloors, Current.Collider.size.y, _shakeStartTime);
            _towerRoot.rotation = Quaternion.Euler(new Vector3(0, 0, currentShakeAngle));
        }
    }

    private static float GetShakeAngleFromTime(int totalFloors, float blockHeight, float shakeStartTime)
    {
        float globalScreenWidth = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, 0, 0)).x - Camera.main.ScreenToWorldPoint(Vector3.zero).x;
        float absoluteMaxTowerHeight = Mathf.Max(totalFloors, TConfig.MAX_SHAKE_FLOORS) * blockHeight;
        float absoluteMaxAngleTan = (globalScreenWidth / 20f) / absoluteMaxTowerHeight;
        int floorHeight = Mathf.Min(TConfig.MAX_SHAKE_FLOORS, totalFloors);
        float rotationAngleLimit = Mathf.Atan(absoluteMaxAngleTan) * Mathf.Rad2Deg * ((float)floorHeight) / ((float)TConfig.MAX_SHAKE_FLOORS);
        return Mathf.Sin((Time.time - shakeStartTime) * TConfig.SHAKE_SPEED) * rotationAngleLimit;
    }    
}
