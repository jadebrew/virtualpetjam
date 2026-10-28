using UnityEngine;
using System.Collections.Generic;
using Audio;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(Button))]
public class ConsumableHelper : MonoBehaviour
{
    [SerializeField] private SongTag assignedSongTitle;
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
            if (item.IsCassette())
            {
                if (pet.inventory[item]>0)
                {
                    assignedSongTitle = itemRef.assignedSongTitle;
                    button.onClick.AddListener(PlayAssignedSong);
                }
            }
            else if (item.IsCoin())
            {
                button.onClick.AddListener(pet.Win);
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
        audio.Play(assignedSongTitle);
    }
}
