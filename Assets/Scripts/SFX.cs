using System;
using UnityEngine;
using UnityEngine.Serialization;

[Serializable]
public enum SFXTag
{
    Annoyed,
    Boop,
    Cancel,
    Eat,
    Hungry,
    MajorVictory,
    MinorVictory,
    NewItem,
    Select,
    SleepingLoop,
    StinkyLoop,
    WakeUp,
    Wash,
};

[CreateAssetMenu(fileName = "SFX", menuName = "Scriptable Objects/SFX")]
public class SFX : ScriptableObject
{

    public string title;
    [SerializeField] public SFXTag tag;
    [SerializeField] public AudioClip audioClip;
}
