using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class SceneManager : MonoBehaviour
{
    public GameState[] gameStateDB;

    [SerializeField]
    public Dictionary<string,GameObject> panelDB = new();

    public GameState currentState;
}
