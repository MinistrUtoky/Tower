using Arsenal;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Assertions;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Tower
{
    internal class ClassicDroppableFactory : AbstractDroppableFactory
    {
        [Header("Gameplay")]
        [SerializeField]
        private ScreenRelation _clickScreenPart = ScreenRelation.Full;
        [SerializeField]
        private Transform _towerRoot;
        [SerializeField]
        private PendulumPhysical _pendulum;

        [Header("UI")]
        [SerializeField]
        private TowerVisualsController _visualsController;

        private float _shakeStartTime;
        private bool _isShaking = false;

        private int _hp = 3;

        private float _awaitStart = TConfig.LOADING_TIME;

        private ArsenalBlock _lastArsenalBlock;
        private int _nextProbabilityIndex = 0;

        private bool ScreenPartTouched => _clickScreenPart == ScreenRelation.Full
                                          || _clickScreenPart == ScreenRelation.BottomHalf & Input.touches[0].position.y > Screen.height / 2f
                                          || _clickScreenPart == ScreenRelation.TopHalf & Input.touches[0].position.y <= Screen.height / 2f;

        protected new void Start()
        {
            base.Start();
            Assert.IsNotNull(TransitionManager.Instance);
            Assert.IsNotNull(_towerRoot);
            Assert.IsNotNull(_pendulum);
            Assert.IsNotNull(_visualsController);
            _visualsController.RedecoratePendulum(InterplayData.Location.PendulumImage, InterplayData.Location.PendulumHolderImage);
            _awaitStart = TConfig.LOADING_TIME;
            SpawnRandomDroppable();
        }

        private void Update()
        {
            if (IsHanging)
                Current.Collider.transform.position = _pendulum.Position;

            if (!TransitionManager.Instance.GameOn) return;

            if (_awaitStart > 0f)
            {
                _awaitStart -= Time.deltaTime;
                return;
            }
            if (IsAlive & IsHanging & (Input.touchCount > 0 || Input.GetMouseButtonDown(0)))
                if (ScreenPartTouched) DropCurrent();

            if (Current != null)
            {
                if (TotalFloors > TConfig.MIN_SHAKE_FLOORS)
                {
                    if (!_isShaking)
                    {
                        _shakeStartTime = Time.time;
                        _isShaking = true;
                    }
                    if (Current.Collider != null)
                        _towerRoot.rotation = TConfig.ShakeAngleFromTime(TotalFloors, Current.Collider.size.y, _shakeStartTime);
                }
            }
        }

        public override void Add(IDroppable towerBlock)
        {
            base.Add(towerBlock);
            towerBlock.Collider.transform.parent.parent = _towerRoot;
            _visualsController.RefreshTower(towerBlock, _towerRoot, TotalFloors);
            _pendulum.SpeedUpPendulum();
        }

        public override void RemoveTopmost()
        {
            base.RemoveTopmost();
            _visualsController.RefreshTower(Current, _towerRoot, TotalFloors);
            _pendulum.SpeedUpPendulum();
        }
        
        public override void TakeHit()
        {
            if (_hp < 1) return;
            _hp -= 1;
            if (_hp < 1) Die();
            else 
                _visualsController.UpdateHP(_hp);
        }
        public override void Heal()
        {
            _hp += 1;
            if (_hp > 2) _hp = 3;
            _visualsController.UpdateHP(_hp);
        }

        public override void Die()
        {
            base.Die();
            _visualsController.CallOnDeathBanner(_clickScreenPart, TotalFloors);
        }

        public async override void SpawnRandomDroppable()
        {
            Vector3 whereToSpawn = _pendulum.Position;
            IBlock block = NextBlock();            
            var prefabHandle = Addressables.InstantiateAsync(block.PrefabAddressable, whereToSpawn, Quaternion.identity);
            var spriteHandle = Addressables.LoadAssetAsync<Sprite>(block.ImageAddressable);

            await Task.WhenAll(prefabHandle.Task, spriteHandle.Task);

            if (prefabHandle.Status != AsyncOperationStatus.Succeeded)
            {
                Debug.LogError($"Failed to load {block.PrefabAddressable}");
                return;
            }
            GameObject spawned = prefabHandle.Result;
            IDroppable droppable = spawned.transform.GetChild(0).GetComponent<IDroppable>();
            if (droppable == null)
            {
                Debug.LogError("Spawned prefab missing IDroppable on child 0");
                Addressables.ReleaseInstance(spawned);
                return;
            }
            if (spriteHandle.Status == AsyncOperationStatus.Succeeded && spriteHandle.Result != null)
            {
                droppable.Image.sprite = spriteHandle.Result;
                droppable.SpriteReleasable.AttachAddressableHandle(spriteHandle);
            }
            else
            {
                Debug.LogWarning($"Failed to load {block.ImageAddressable}");
                spriteHandle.IsValid(); 
                Addressables.Release(spriteHandle);
            }
            base.SpawnDroppable(droppable);
        }

        private IBlock NextBlock()
        {
            if (_lastArsenalBlock == null)
            {
                _nextProbabilityIndex = 0;
                _lastArsenalBlock = InterplayData.Arsenal.NextBlock();
                _visualsController.NewProbability(_lastArsenalBlock.ProbabilitiesByTurn[_nextProbabilityIndex]);
                _visualsController.NewArsenalBlock(_lastArsenalBlock.ImageAddressable);
                return InterplayData.Location.NextBlock();
            }
            if (_lastArsenalBlock.ProbabilitiesByTurn.Length == 0) return InterplayData.Location.NextBlock();

            IBlock block;
            if (Random.Range(0f, 1f) < _lastArsenalBlock.ProbabilitiesByTurn[_nextProbabilityIndex])
            {
                block = _lastArsenalBlock;
                _lastArsenalBlock = InterplayData.Arsenal.NextBlock();
                _nextProbabilityIndex = 0;
                _visualsController.NewProbability(_lastArsenalBlock.ProbabilitiesByTurn[_nextProbabilityIndex]);
                _visualsController.NewArsenalBlock(_lastArsenalBlock.ImageAddressable);
            }
            else
            {
                _nextProbabilityIndex = Mathf.Min(_nextProbabilityIndex + 1, _lastArsenalBlock.ProbabilitiesByTurn.Length - 1);
                _visualsController.NewProbability(_lastArsenalBlock.ProbabilitiesByTurn[_nextProbabilityIndex]);
                block = InterplayData.Location.NextBlock();
            }


            return block;
        }
    }
}