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

    public void BuildAlias()
    {
        float[] weights = new float[Blocks.Count()];
        for (int i = 0; i < weights.Length; i++)
            weights[i] = Blocks.ElementAt(i).ProbabilityWeight;
        _prob = new float[weights.Length];
        _alias = new int[weights.Length];
        unsafe { BuildAliasTable(weights, _prob, _alias); }
    }

    public override LocationBlock NextBlock()
    {
        int index = AliasDraw(_prob, _alias, new System.Random());
        return _locationBlocks[index];
    }
}
