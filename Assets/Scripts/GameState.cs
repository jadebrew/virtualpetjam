using UnityEngine;

[CreateAssetMenu(fileName = "GameState", menuName = "Scriptable Objects/GameState")]
public class GameState : ScriptableObject
{

    public string title;

    public string body;

    public GameState[] buttons;

    public string[] panels;
}
