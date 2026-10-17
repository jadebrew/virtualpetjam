using System;
using UnityEngine;
using UnityEngine.Serialization;

public enum ItemType {
    Consumable,
    Cassette,
    Coin
};

[CreateAssetMenu(fileName = "Item", menuName = "Scriptable Objects/Item")]
public class Item : ScriptableObject
{

    public Sprite sprite;
    public string title;
    [SerializeField] public ItemType type;
    
    
    [SerializeField] public NeedType needModifier = NeedType.None;
    [SerializeField] public float needModValue = 0.0f;

    public Rarity rarity;

    [SerializeField] public SFXTag onUseSFX;
    
    [SerializeField] public SongTag assignedSongTitle;

    public string itemType;

    public bool IsConsumable()
    {
        return type == ItemType.Consumable;
    }
    
    public bool IsCassette()
    {
        return type == ItemType.Cassette;
    }
}
