using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class LcdHelper : MonoBehaviour
{
    public TMP_Text LcdPanel;
    private AudioManager _audio;

    void Awake()
    {
        _audio = AudioManager.Instance;
    }

    void Update()
    {
        var status = "Loading...";
        if (_audio == null)
        {
            _audio = AudioManager.Instance;
        }
        var currentSong = _audio.NowPlaying();
        if(!_audio.IsPaused())
        {
            var time = _audio.GetTime();
            var length = _audio.GetLength();

            int ts = ((int)time % 60);
            int tm = ((int)time / 60);

            int ls = ((int)length % 60);
            int lm = ((int)length / 60);

            var timeleft = length - time;

            int seconds = ((int)timeleft % 60);
            int minutes = ((int)timeleft / 60);

            status = currentSong.artist + " - " + currentSong.title + " (" + string.Format("{0:00}:{1:00}", minutes,seconds) + ")";
        } else {
            status = "Paused";
        }


        LcdPanel.text = status;
    }
}
