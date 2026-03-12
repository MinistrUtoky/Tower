using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class LocationBlock : Block
{
    [SerializeField]
    private float _probabilityWeight;
    public float ProbabilityWeight => _probabilityWeight;
}

[CreateAssetMenu(fileName = "LocationPresetScriptable", menuName = "Scriptable Objects/LocationPresetScriptable")]
public class LocationPresetScriptable : AbstractPresetScriptable<LocationBlock>
{
    [SerializeField]
    private Sprite _pendulumHolderImage;
    [SerializeField]
    private Sprite _pendulumImage;
    [SerializeField]
    private bool _reverseOverlap;


    [SerializeField]
    private LocationBlock[] _locationBlocks;

    public Sprite PendulumHolderImage => _pendulumHolderImage;
    public Sprite PendulumImage => _pendulumImage;
    public bool ReverseOverlap => _reverseOverlap;

    public override IEnumerable<LocationBlock> Blocks => _locationBlocks;

    public override LocationBlock NextBlock() 
    {
        if (_locationBlocks.Length == 0)
            return new LocationBlock();
        float totalWeight = _locationBlocks.Sum(x => x.ProbabilityWeight) + 1,
              idx = UnityEngine.Random.Range(0f, 1f) * totalWeight;
        Debug.Log("Block spawn value = " + idx + " with total weight being = " + _locationBlocks.Sum(x => x.ProbabilityWeight));
        float sum = 0;
        int i = 0;
        while (i < _locationBlocks.Length) {
            if (sum > idx)
                break;
            sum += _locationBlocks.ElementAt(i).ProbabilityWeight;
            i++;
        }
        Debug.Log("Selected index = " + i);
        if (i == 0 || i > _locationBlocks.Length) 
            i = 1;
        return _locationBlocks[i - 1];
    }
}
