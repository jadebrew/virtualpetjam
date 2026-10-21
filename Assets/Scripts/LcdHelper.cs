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
        if(!_audio.IsPaused())
        {
            var time = _audio.GetTime();
            var length = _audio.GetLength();

            int ts = ((int)time % 60);
            int tm = ((int)time / 60);

            int ls = ((int)length % 60);
            int lm = ((int)length / 60);

            int seconds = ls-ts;
            int minutes = lm-tm;

            status = string.Format("{0:00}:{1:00}/{2:00}:{3:00}", tm,ts,lm,ls);
        } else {
            status = "Paused";
        }


        LcdPanel.text = status;
    }
}
