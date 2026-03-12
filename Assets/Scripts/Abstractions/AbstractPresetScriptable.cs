using System;
using System.Collections.Generic;
using UnityEngine;

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


[CreateAssetMenu(fileName = "AbstractPresetScriptable", menuName = "Scriptable Objects/AbstractPresetScriptable")]
public abstract class AbstractPresetScriptable<T> : ScriptableObject where T : IBlock
{
    public abstract IEnumerable<T> Blocks { get; }
    public abstract T NextBlock();
}
