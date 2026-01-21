using System;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "SkinsetScriptable", menuName = "Scriptable Objects/SkinsetScriptable")]
public class BlockPresetScriptable : ScriptableObject
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

        public readonly string Name => _name;
        public readonly Sprite Image => _image;
        public readonly GameObject Prefab => _prefab;

        public readonly float ProbabilityWeight => _probabilityWeight;
    }

    [SerializeField]
    private Sprite _pendulumHolderImage;
    [SerializeField]
    private Sprite _pendulumImage;
    [SerializeField]
    private Block[] _blocks;
    [SerializeField]
    private bool _reverseOverlap;

    public Sprite PendulumHolderImage => _pendulumHolderImage;
    public Sprite PendulumImage => _pendulumImage;
    public bool ReverseOverlap => _reverseOverlap;

    internal Block RandomBlock()
    {
        if (_blocks.Length == 0)
        {
            Debug.LogWarning("Assert does not contain any blocks!");
            return new Block();
        }
        float idx = UnityEngine.Random.Range(0f, 1f) * (_blocks.Sum(x => x.ProbabilityWeight)+1);
        Debug.Log("Block spawn value = " + idx + " with total weight being = " + _blocks.Sum(x => x.ProbabilityWeight));
        float sum = 0;
        int i = 0;
        while (i < _blocks.Length) {
            if (sum > idx)
                break;
            sum += _blocks[i].ProbabilityWeight;
            i++;
        }
        Debug.Log("Selected index = " + i);
        if (i == 0 || i > _blocks.Length) 
            i = 1;
        return _blocks[i - 1];
    }
}
