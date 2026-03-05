using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "LocationPresetScriptable", menuName = "Scriptable Objects/LocationPresetScriptable")]
internal class LocationPresetScriptable : AbstractPresetScriptable
{
    [SerializeField]
    private Sprite _pendulumHolderImage;
    [SerializeField]
    private Sprite _pendulumImage;
    [SerializeField]
    private bool _reverseOverlap;

    public Sprite PendulumHolderImage => _pendulumHolderImage;
    public Sprite PendulumImage => _pendulumImage;
    public bool ReverseOverlap => _reverseOverlap;

    public override Block NextBlock() => RandomBlock();
    private Block RandomBlock()
    {
        if (_blocks.Length == 0)
            return new Block();
        float totalWeight = _blocks.Sum(x => x.ProbabilityWeight) + 1,
              idx = Random.Range(0f, 1f) * totalWeight;
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
