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
    public bool isNewGame;//TODO this is awful
    public bool isStart;//TODO this is awful
    public bool isGame;//TODO this is awful
}
