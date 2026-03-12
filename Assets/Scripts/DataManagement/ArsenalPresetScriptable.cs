using System;
using UnityEngine;
using System.IO;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Linq;

namespace Arsenal
{
    // + Арсенал блоков. Игрок выбирает набор блоков - эти блоки будут падать игроку, даже если их нет на локации.
    // + Индикатор арсенала. UI указатель следующего блока в арсенале, растущая с ходами вероятность его появления с круговым заполнением.
    // + Меню выбора арсенала. Содержит блоки для сборки "руки", их описание и график роста вероятности выпадения с ходами. 
    // Планируется сквозная прогрессия за очки, как с уровнями.

    [Serializable]
    public class ArsenalBlock : Block
    {
        [SerializeField]
        private bool _isSelected = false;
        [SerializeField]
        private float _price;
        [SerializeField]
        private float[] _probabilitiesByTurn = new float[] { 0.1f, 0.2f, 0.4f, 0.7f, 1f };

        public float[] ProbabilitiesByTurn => _probabilitiesByTurn;
        public bool IsSelected { get { return _isSelected; } set { _isSelected = value; } }
    }

    [CreateAssetMenu(fileName = "ArsenalPresetScriptable", menuName = "Scriptable Objects/ArsenalPresetScriptable")]
    public class ArsenalPresetScriptable : AbstractPresetScriptable<ArsenalBlock>
    {
        [Serializable]
        private class ArrayWrapper<T> {
            [SerializeField] private T[] _array;
            public T[] Array => _array;
            public ArrayWrapper(T[] arr) => _array = arr;            
        }

        // Decorations for probability circle
        //[SerializeField]
        //private Sprite _probabilityBackground;
        //[SerializeField]
        //private TMP_Settings _probabilityTextSettings;
        [SerializeField]
        private ArsenalUIBlock _UIBlock;
        [SerializeField]
        private ArsenalUIBlock _UIBlockMini;

        [SerializeField]
        private ArsenalBlock[] _arsenalBlocks;

        public override IEnumerable<ArsenalBlock> Blocks => _arsenalBlocks;
        public override ArsenalBlock NextBlock() {
            IEnumerable<ArsenalBlock> selected = _arsenalBlocks.Where(x => x.IsSelected);
            return selected.ElementAt(UnityEngine.Random.Range(0, selected.Count())); 
        }
        
        public void FillUpContent(Transform shopSection, Transform inventorySection)
        {
            LoadArsenalPreset();
            foreach (ArsenalBlock block in _arsenalBlocks)
            {
                ArsenalUIBlock mainUIBlock = Instantiate(_UIBlock.gameObject, shopSection).GetComponent<ArsenalUIBlock>();
                ArsenalUIBlock miniUIBlock = Instantiate(_UIBlockMini.gameObject, inventorySection).GetComponent<ArsenalUIBlock>();
                miniUIBlock.gameObject.SetActive(block.IsSelected);
                mainUIBlock.GetComponent<Button>().enabled = !block.IsSelected;
                miniUIBlock.Init(() => Switch(block, mainUIBlock, miniUIBlock), block.Image, block.Name, block.ProbabilitiesByTurn);
                mainUIBlock.Init(() => Switch(block, mainUIBlock, miniUIBlock),  block.Image, block.Name, block.ProbabilitiesByTurn);
            }
        }

        private static void Switch(ArsenalBlock realBlock, ArsenalUIBlock mainBlock, ArsenalUIBlock miniBlock)
        {
            realBlock.IsSelected = !realBlock.IsSelected;
            miniBlock.gameObject.SetActive(realBlock.IsSelected);
            mainBlock.GetComponent<Button>().enabled = !realBlock.IsSelected;
            Canvas.ForceUpdateCanvases();
        }

        private void LoadArsenalPreset()
        {
            string path = Path.Combine(Application.streamingAssetsPath, MConfig.ARSENAL_SAVE_FILE_NAME);
            if (!File.Exists(path)) SavePresetAsJson();
            string json = File.ReadAllText(path);
            ArrayWrapper<ArsenalBlock> arsenal = JsonUtility.FromJson<ArrayWrapper<ArsenalBlock>>(json);
            _arsenalBlocks = arsenal.Array;
        }

        public void SavePresetAsJson()
        {
            string path = Path.Combine(Application.streamingAssetsPath, MConfig.ARSENAL_SAVE_FILE_NAME); ;
            string json = JsonUtility.ToJson(new ArrayWrapper<ArsenalBlock>(_arsenalBlocks));
            File.WriteAllText(path, json);
        }
    }
}