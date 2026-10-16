using UnityEngine;

[CreateAssetMenu(fileName = "GameState", menuName = "Scriptable Objects/GameState")]
public class GameState : ScriptableObject
{

    public string title;

    public string shortname;

    public string body;

    public GameState[] buttons;

    public string[] panels;

    public bool isBag;
    public string bagFilter;
}
