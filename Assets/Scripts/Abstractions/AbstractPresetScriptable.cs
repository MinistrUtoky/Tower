using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AbstractPresetScriptable", menuName = "Scriptable Objects/AbstractPresetScriptable")]
internal abstract class AbstractPresetScriptable : ScriptableObject
{
    [Serializable]
    internal struct Block
    {
        [SerializeField]
        private string _name;
        [SerializeField]
        private Sprite _image;
        [SerializeField]
        private GameObject _prefab;
        [SerializeField]
        private float _probabilityWeight;
        [SerializeField]
        private float[] _probabilitiesByTurn;

        public readonly string Name => _name;
        public readonly Sprite Image => _image;
        public readonly GameObject Prefab => _prefab;
        public readonly float ProbabilityWeight => _probabilityWeight;
        public readonly float[] ProbabilitiesByTurn => _probabilitiesByTurn;
    }
    [SerializeField]
    protected Block[] _blocks;

    public IEnumerable<Block> Blocks => _blocks;

    public abstract Block NextBlock();
}
