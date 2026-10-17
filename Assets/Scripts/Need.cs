using System;
using UnityEngine;
using UnityEngine.Serialization;

public enum NeedType {
    None,
    Hygiene,
    Sleep,
    Hunger,
};

[CreateAssetMenu(fileName = "Need", menuName = "Scriptable Objects/Need")]
public class Need : ScriptableObject
{

    public string title;
    public NeedType type;
    public float value;
}
