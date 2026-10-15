using UnityEngine;
using static AudioManager.SFXTitle;

[CreateAssetMenu(fileName = "Item", menuName = "Scriptable Objects/Item")]
public class Item : ScriptableObject
{

    public Sprite sprite;
    public string title;

    public bool isConsumable;
    public string needModifier;
    public int needModValue;

    public Rarity rarity;

    [SerializeField] public AudioManager.SFXTitle onUseSFX;

    public bool isCasette;
    [SerializeField] public AudioManager.SongTitle assignedSongTitle;

    public string itemType;
}
