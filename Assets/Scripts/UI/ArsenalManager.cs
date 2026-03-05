using UnityEngine;
using UnityEngine.UI;
using static System.Collections.Specialized.BitVector32;

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
        private ArsenalPresetScriptable _arsenalShopPreset;
        [SerializeField]
        private ArsenalPresetScriptable _yourArsenalPreset;

        private void Awake()
        {
            FillSectionContents(_arsenalShopContent, _arsenalShopPreset);
            FillSectionContents(_yourArsenalContent, _yourArsenalPreset);
            Canvas.ForceUpdateCanvases();
        }

        public void CloseBlockInfo() => SetActive(_infoPopup.gameObject, false);
        public void OpenBlockInfo() => SetActive(_infoPopup.gameObject, true);

        private void FillSectionContents(Transform section, ArsenalPresetScriptable preset)
        {
            foreach (AbstractPresetScriptable.Block block in preset.Blocks)
            {
                ArsenalUIBlock uiBlock = Instantiate(preset.UIPrefab, section).GetComponent<ArsenalUIBlock>();
                uiBlock.Init(this, block.Image, block.Name, block.ProbabilitiesByTurn);
            }
        }

        public void SwitchInArsenal(ArsenalUIBlock uiBlock) {
            if (!uiBlock) return; 
            if (uiBlock.transform.parent == _yourArsenalContent)
            {
                DestroyImmediate(uiBlock.gameObject);
            }
            else
            {
                ArsenalUIBlock newUIBlock = Instantiate(_yourArsenalPreset.UIPrefab, _yourArsenalContent).GetComponent<ArsenalUIBlock>();
                newUIBlock.Init(this, uiBlock.Icon, uiBlock.Name, uiBlock.PerTurnProbabilities);
            }
            Canvas.ForceUpdateCanvases();
        }
    }

}