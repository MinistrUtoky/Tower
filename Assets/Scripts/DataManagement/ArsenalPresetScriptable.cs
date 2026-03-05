using TMPro;
using UnityEngine;

namespace Arsenal
{
    [CreateAssetMenu(fileName = "ArsenalPresetScriptable", menuName = "Scriptable Objects/ArsenalPresetScriptable")]
    internal class ArsenalPresetScriptable : AbstractPresetScriptable
    {
        // + Арсенал блоков. Игрок выбирает набор блоков - эти блоки будут падать игроку, даже если их нет на локации.
        // + Индикатор арсенала. UI указатель следующего блока в арсенале, растущая с ходами вероятность его появления с круговым заполнением.
        // + Меню выбора арсенала. Содержит блоки для сборки "руки", их описание и график роста вероятности выпадения с ходами. 
        // Планируется сквозная прогрессия за очки, как с уровнями.


        // Decorations for probability circle
        [SerializeField]
        private Sprite _probabilityBackground;
        [SerializeField]
        private TMP_Settings _probabilityTextSettings;
        [SerializeField]
        private ArsenalUIBlock _UIBlock;

        public GameObject UIPrefab => _UIBlock.gameObject;

        public override Block NextBlock() => _blocks[Random.Range(0, _blocks.Length)];
    }
}