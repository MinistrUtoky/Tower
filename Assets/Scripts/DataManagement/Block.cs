using System;
using UnityEngine;
using UnityEngine.AddressableAssets;


[Serializable]
public class Block : IBlock
{
    [SerializeField]
    private string _name;
    [SerializeField]
    private AssetReferenceSprite _image;
    [SerializeField]
    private AssetReferenceGameObject _prefab;

    public string Name => _name;
    public string ImageAddressable => _image.RuntimeKey.ToString();
    public string PrefabAddressable => _prefab.RuntimeKey.ToString();

    public Block(string name = "", string imageAddressableKey = "", string prefabAddressableKey = "")
    {
        _name = name;
        _image = new AssetReferenceSprite(imageAddressableKey);
        _prefab = new AssetReferenceGameObject(prefabAddressableKey);
    }
}
