using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;

namespace Arsenal
{
    public class ArsenalManager : AbstractPanelManager
    {
        [SerializeField]
        private Image _infoPopup;
        [SerializeField]
        private Transform _arsenalShopContent;
        [SerializeField]
        private Transform _yourArsenalContent;

        [SerializeField]
        private ArsenalUIBlock _UIBlock;
        [SerializeField]
        private ArsenalUIBlock _UIBlockMini;

        [SerializeField]
        private ArsenalPresetScriptable _arsenalPreset;

        public static ArsenalPresetScriptable ArsenalPreset { get; private set; }
        
        private void Awake()
        {
            FillUpContentAsync(_arsenalShopContent, _yourArsenalContent);
            ArsenalPreset = _arsenalPreset;
            Canvas.ForceUpdateCanvases();
        }

        public void Save() => _arsenalPreset.SavePresetAsBinary();  //_arsenalPreset.SavePresetAsJson(); 

        public void CloseBlockInfo() => SetActive(_infoPopup.gameObject, false);
        public void OpenBlockInfo() => SetActive(_infoPopup.gameObject, true);

        public async Task FillUpContentAsync(Transform shopSection, Transform inventorySection)
        {
            //_arsenalPreset.LoadArsenalPreset();
            _arsenalPreset.LoadPresetBinary();
            var loadTasks = _arsenalPreset.Blocks.Select(async block =>
            {
                if (string.IsNullOrEmpty(block.ImageAddressable))
                    return (block, Image: (Sprite)null, Handle: default(AsyncOperationHandle));
                var handle = Addressables.LoadAssetAsync<Sprite>(block.ImageAddressable);
                await handle.Task;
                return handle.Status == AsyncOperationStatus.Succeeded ? (block, handle.Result, handle)
                                                                         : (block, null, handle);
            });
            var results = await Task.WhenAll(loadTasks);
            foreach (var (block, sprite, handle) in results)
            {
                if (sprite == null) continue;
                ArsenalUIBlock main = Instantiate(_UIBlock.gameObject, shopSection).GetComponent<ArsenalUIBlock>();
                ArsenalUIBlock mini = Instantiate(_UIBlockMini.gameObject, inventorySection).GetComponent<ArsenalUIBlock>();
                mini.gameObject.SetActive(block.IsSelected);
                main.GetComponent<Button>().enabled = !block.IsSelected;
                mini.Init(() => Switch(block, main, mini), sprite, block.Name, block.ProbabilitiesByTurn);
                main.Init(() => Switch(block, main, mini), sprite, block.Name, block.ProbabilitiesByTurn);
                main.SpriteReleasable.AttachAddressableHandle(handle);
            }
        }

        private static void Switch(ArsenalBlock realBlock, ArsenalUIBlock mainBlock, ArsenalUIBlock miniBlock)
        {
            realBlock.IsSelected = !realBlock.IsSelected;
            miniBlock.gameObject.SetActive(realBlock.IsSelected);
            mainBlock.GetComponent<Button>().enabled = !realBlock.IsSelected;
            Canvas.ForceUpdateCanvases();
        }

    }

}