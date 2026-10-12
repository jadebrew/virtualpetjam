using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Android;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }
    
    [SerializeField] private AudioSource _audioSfx;
    [SerializeField] private AudioSource _audioBg;
    
    [Serializable]
    public enum SongTitle {
        EnergyDash,
        JapanInverted,
        Snow,
        Sunset,
        Bamboo,
        Japan,
        Night,
        Zebra
    };

    [Serializable]
    public enum SFXTitle
    {
        Annoyed,
        Boop,
        Cancel,
        Eat,
        Hungry,
        MajorVictory,
        MinorVictory,
        NewItem,
        Select,
        SleepingLoop,
        StinkyLoop,
        WakeUp,
        Wash,
    };
    
    [Serializable]
    public struct SongMapping
    {
        public SongTitle title;
        public AudioClip audioClip;
    }
    
    [Serializable]
    public struct SfxMapping
    {
        public SFXTitle title;
        public AudioClip audioClip;
    }
    
    [SerializeField] private List<SongMapping> songsDb = new ();
    [SerializeField] private List<SfxMapping> sFXDb = new ();
    
    private Dictionary<SongTitle, AudioClip> _songDict = new ();
    private Dictionary<SFXTitle, AudioClip> _sfxDict = new ();
    
    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        
        foreach (var entry in songsDb)
        {
            if (!_songDict.ContainsKey(entry.title))
            {
                _songDict.Add(entry.title, entry.audioClip);
            }
            else
            {
                Debug.LogWarning($"Duplicate song entry for: {entry.title}");
            }
        }
        
        foreach (var entry in sFXDb)
        {
            if (!_sfxDict.ContainsKey(entry.title))
            {
                _sfxDict.Add(entry.title, entry.audioClip);
            }
            else
            {
                Debug.LogWarning($"Duplicate song entry for: {entry.title}");
            }
        }
    }

    public void Play(SongTitle title)
    {
        Play(GetSong(title), _audioBg);
    }
    
    public void Play(SFXTitle title)
    {
        Play(GetSong(title), _audioSfx);
    }

    public void StopSound()
    {
        _audioSfx.Stop();
    }

    public void StopMusic()
    {
        _audioBg.Stop();
    }

    private AudioClip GetSong(SongTitle title)
    {
        if (_songDict.TryGetValue(title, out AudioClip clip))
        {
            return clip;
        }
        Debug.LogWarning($"Song not found: {title}");
        return null;
    }
    
    private AudioClip GetSong(SFXTitle title)
    {
        if (_sfxDict.TryGetValue(title, out AudioClip clip))
        {
            return clip;
        }
        Debug.LogWarning($"SFX not found: {title}");
        return null;
    }

    private void Play(AudioClip clip, AudioSource source)
    {
        source.clip = clip;
        source.Play();
    }
    
}
