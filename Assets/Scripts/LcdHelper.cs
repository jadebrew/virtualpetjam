using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class LcdHelper : MonoBehaviour
{
    public TMP_Text LcdPanel;
    private AudioManager _audio;

    void FixedUpdate()
    {
        if(!_audio)
        {
             _audio = AudioManager.Instance;
        }
        LcdPanel.text = _audio.Status() + " || " + _audio.NowPlaying().artist + " - " + _audio.NowPlaying().title;
    }
}
