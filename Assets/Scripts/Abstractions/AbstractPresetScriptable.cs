using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AbstractPresetScriptable", menuName = "Scriptable Objects/AbstractPresetScriptable")]
internal abstract class AbstractPresetScriptable : ScriptableObject
{
    public interface IBlock
    {
        public string Name { get; }
        public Sprite Image { get; }
        public GameObject Prefab { get; }
    }

    [Serializable]
    public class Block : IBlock
    {
        [SerializeField]
        private string _name;
        [SerializeField]
        private Sprite _image;
        [SerializeField]
        private GameObject _prefab;
        public string Name => _name;
        public Sprite Image => _image;
        public GameObject Prefab => _prefab;
    }

    public abstract IEnumerable<IBlock> Blocks { get; }
    public abstract IBlock NextBlock();
}
