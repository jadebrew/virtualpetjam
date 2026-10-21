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
        songText.text =  _audio.Status() + "\n" + _audio.NowPlaying().artist + "\n" + _audio.NowPlaying().title + "\n";
        albumCover.sprite = _audio.NowPlaying().sprite;
        clockText.text = System.DateTime.Now.ToString("HH:mm\ndd MMMM\nyyyy");
    }
}
