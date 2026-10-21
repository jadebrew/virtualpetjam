using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;

public class TimeHelper : MonoBehaviour
{
    public TMP_Text clockText;
    public Image albumCover;
    public TMP_Text songText;
    private AudioManager _audio;


    // Update is called once per frame
    void Update()
    {
        if(!_audio)
        {
            _audio = AudioManager.Instance;
        }
        songText.text = _audio.NowPlaying().artist + " - " + _audio.NowPlaying().title + "\n" + _audio.Status();
        albumCover.sprite = _audio.NowPlaying().sprite;
        clockText.text = System.DateTime.UtcNow.ToString("MMMM dd\nHH:mm");
    }
}
