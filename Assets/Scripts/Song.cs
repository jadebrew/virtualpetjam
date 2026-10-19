using System;
using UnityEngine;

[Serializable]
public enum SongTag {
    EnergyDash,
    JapanInverted,
    Snow,
    Sunset,
    Bamboo,
    Japan,
    Night,
    Zebra,
    Clouds,
	DeepSea,
};

[CreateAssetMenu(fileName = "Song", menuName = "Scriptable Objects/Song")]
public class Song : ScriptableObject
{

    public Sprite sprite;
    public string title;
    [SerializeField] public SongTag tag;
    [SerializeField] public AudioClip audioClip;
}
