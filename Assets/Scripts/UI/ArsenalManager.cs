using UnityEngine;
using UnityEngine.UI;
using static AbstractPresetScriptable;

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
        private ArsenalPresetScriptable _arsenalPreset;
        
        private void Awake()
        {
            _arsenalPreset.FillUpContent(_arsenalShopContent, _yourArsenalContent);
            Canvas.ForceUpdateCanvases();
        }

        public void Save() => _arsenalPreset.SavePresetAsJson();


        public void CloseBlockInfo() => SetActive(_infoPopup.gameObject, false);
        public void OpenBlockInfo() => SetActive(_infoPopup.gameObject, true);

    }

}