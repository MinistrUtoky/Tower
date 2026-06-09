using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;

namespace Tower {
    internal class TowerVisualsController : MonoBehaviour
    {
        [Header("UI")]
        [SerializeField]
        private TMP_Text _score;
        [SerializeField]
        private Image[] _hearts;
        [SerializeField]
        private Image _pendulumVisual;
        [SerializeField]
        private Image _pendulumHolderVisual;
        [SerializeField]
        private Image _arsenalNextBlockVisual;
        [SerializeField]
        private TMP_Text _arsenalNextBlockChance;

        [Header("Endgame")]
        [SerializeField]
        private GameObject _banner;
        [SerializeField]
        private TMP_Text _endgameResult;

        [Header("Other")]
        [SerializeField]
        private Camera _cameraToSpawn;
        [SerializeField]
        private Canvas _background;
        
        private float _currentScreenWidthCoef = 1f;
        private float _baseCameraOrthoWidth;

        private void Awake()
        {
            _baseCameraOrthoWidth = _cameraToSpawn.orthographicSize;
        }

        public void RedecoratePendulum(Sprite newPendulum, Sprite newPendulumHolder)
        {
            _pendulumVisual.sprite = newPendulum; 
            _pendulumHolderVisual.sprite = newPendulumHolder;
        }

        public void UpdateHP(int hp)
        {
            for (int i = 0; i < _hearts.Length; i++) _hearts[i].gameObject.SetActive(false);
            for (int i = 0; i < hp; i++)
            {
                _hearts[i].gameObject.SetActive(true);
                _hearts[i].DOKill();
                _hearts[i].DOFade(0, 0.2f).SetEase(Ease.Linear).SetLoops(4, LoopType.Yoyo);
            }
        }

        public void CallOnDeathBanner(ClassicDroppableFactory.ScreenRelation screenPart, int totalFloors)
        {
            if (screenPart == ClassicDroppableFactory.ScreenRelation.TopHalf)
                InterplayData.Player2Score = totalFloors;
            else
                InterplayData.Player1Score = totalFloors;
            if (_endgameResult != null)
                _endgameResult.text = totalFloors.ToString();
            TransitionManager.Instance.KillPlayer(_banner, 1f);
        }

        public void RefreshTower(IDroppable towerBlock, Transform towerRoot, int totalFloors)
        {
            _currentScreenWidthCoef = Mathf.Min(TConfig.MAX_SCREEN_WIDTH_COEF,
                                                    _currentScreenWidthCoef + TConfig.SCREEN_WIDTH_INCREMENT);
            float newHeight = -towerBlock.Collider.size.y 
                                * towerBlock.Collider.transform.localScale.y 
                                    * (totalFloors > 1 ? totalFloors - 2 : 0);

            if (totalFloors > TConfig.BACKGROUND_BLOCK_HEIGHT_LIMIT)
            {
                float newBackgroundHeight = towerBlock.Collider.size.y
                                                * towerBlock.Collider.transform.localScale.y
                                                    * (totalFloors - TConfig.BACKGROUND_BLOCK_HEIGHT_LIMIT);
                _background.transform.parent.DOKill();
                _background.transform.DOLocalMoveY(newBackgroundHeight, 1f);
            }
            towerRoot.transform.parent.DOKill();
            towerRoot.transform.parent.DOLocalMoveY(newHeight, 1f);
            _score.text = totalFloors.ToString(); 
            _cameraToSpawn.DOKill();
            _cameraToSpawn.DOOrthoSize(_currentScreenWidthCoef * _baseCameraOrthoWidth, 1f);
        }

        public async void NewArsenalBlock(string addressableKey)
        {
            var handle = Addressables.LoadAssetAsync<Sprite>(addressableKey);
            await handle.Task;
            if (handle.Status == AsyncOperationStatus.Succeeded)
                _arsenalNextBlockVisual.sprite = handle.Result;
        }
        public void NewProbability(float currentProbability) => _arsenalNextBlockChance.text = (currentProbability * 100).ToString() + "%";
    }
}