using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class MimiSceneManager : MonoBehaviour
{
    public GameState[] gameStateDB;

    [SerializeField]
    public Dictionary<string,GameObject> panelDB = new();

    public GameState currentState;
}
