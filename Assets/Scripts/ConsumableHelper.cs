using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class ConsumableHelper : MonoBehaviour
{
    [SerializeField] private AudioManager.SongTitle assignedSongTitle;
    public Item item;

    private Button button;
    private Petmanager pet;

    public AudioManager audio;

    public void Init(Item itemRef, string filter)
    {
        item = itemRef;
        pet = GameObject.Find("Petmanager").GetComponent<Petmanager>();;
        button = GetComponent<Button>();
        if(filter != "all")
        {
            if (item.isCasette)
            {
                if (pet.inventory[item]>0)
                {
                    assignedSongTitle = itemRef.assignedSongTitle;
                    button.onClick.AddListener(PlayAssignedSong);
                }
            }
            else
            {
                button.onClick.AddListener(Consume);
            }
        }
        audio = AudioManager.Instance;
    }

    void Consume()
    {
        pet.Feed(button.gameObject);
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
