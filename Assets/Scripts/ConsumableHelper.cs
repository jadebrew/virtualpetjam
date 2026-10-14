using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class ConsumableHelper : MonoBehaviour
{
    [SerializeField] private AudioManager.SongTitle assignedSongTitle;
    public int itemValue;

    private Button button;
    private Petmanager pet;

    public string filtertype;
    public AudioManager audio;

    void Awake()
    {
        pet = GameObject.Find("Petmanager").GetComponent<Petmanager>();;
        button = GetComponent<Button>();
        if (filtertype == "casette")
        {
            button.onClick.AddListener(PlayAssignedSong);
        }
        else
        {
           button.onClick.AddListener(Consume);
        }
        audio = AudioManager.Instance;
    }

    void Consume()
    {
        pet.Feed(itemValue);
        Debug.Log("Using a consumable");
    }

    void StopMusic()
    {
        audio.StopMusic();
    }

    void PlayAssignedSong()
    {
        Debug.LogWarning($"PlayAssignedSong: {assignedSongTitle}");
        audio.Play(assignedSongTitle);
    }
}
