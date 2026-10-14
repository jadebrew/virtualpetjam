using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class SongButtonHelper : MonoBehaviour
{
    [SerializeField] private AudioManager.SongTitle assignedSongTitle;
    private Button button;

    public AudioManager audio;

    void Awake()
    {
        audio = AudioManager.Instance;
        button = GetComponent<Button>();
        button.onClick.AddListener(PlayAssignedSong);
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
