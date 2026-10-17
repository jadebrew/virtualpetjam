using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "Rarity", menuName = "Scriptable Objects/Rarity")]
public class Rarity : ScriptableObject
{
    public Sprite sprite;
    public string title;
    public int amountInBanner;
    [SerializeField] public SFXTag winSFX;
}
