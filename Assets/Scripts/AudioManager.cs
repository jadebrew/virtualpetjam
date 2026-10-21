using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Android;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    public float maxvolume = 0.7F;
    public float fadespeed = 0.1F;
    bool fading = false;
    
    [SerializeField] private AudioSource _audioSfx;
    [SerializeField] private AudioSource _audioBg;
    
    private Dictionary<SongTag, Song> _songDict = new ();
    private Dictionary<SFXTag, SFX> _sfxDict = new ();

    private Song _currentSong;

    public void AdjustVolume(float step)
    {
        maxvolume += step;
        if(maxvolume < 0) {maxvolume = 0;}
        if(maxvolume > 1) {maxvolume = 1;}

        if (!fading)
        {
            _audioBg.volume = maxvolume;
        }
    }
    
    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        
        foreach (var song in Resources.LoadAll<Song>("Songs"))
        {
            if (!_songDict.ContainsKey(song.tag))
            {
                _songDict.Add(song.tag, song);
            }
            else
            {
                Debug.LogWarning($"Duplicate song entry for: {song.title}");
            }
        }
        
        foreach (var sound in Resources.LoadAll<SFX>("SFX"))
        {
            if (!_sfxDict.ContainsKey(sound.tag))
            {
                _sfxDict.Add(sound.tag, sound);
            }
            else
            {
                Debug.LogWarning($"Duplicate sound entry for: {sound.title}");
            }
        }
    }

    void Update()
    {
        _audioBg.volume += Time.deltaTime * fadespeed;
        if (_audioBg.volume > maxvolume)
        {
            _audioBg.volume = maxvolume;
            fading = false;
        }
    }

    public void Play(SongTag tag)
    {
        StopMusic();
        _currentSong = GetSong(tag);
        
        Play(_currentSong.audioClip, _audioBg);
        _audioBg.volume = 0;
        fading = true;
        
        NotificationManager.Instance.NotifySong(_currentSong);
    }
    
    public void Play(SFXTag tag)
    {
        Play(GetSound(tag).audioClip, _audioSfx);
    }

    public void StopSound()
    {
        _audioSfx.Stop();
    }

    public float GetTime()
    {
       return _audioBg.time;
    }

    public float GetLength()
    {
        return _audioBg.clip.length;
    }

    public Song NowPlaying()
    {
        return _currentSong;
    }

    public bool IsPaused()
    {
        return !_audioBg.isPlaying;
    }

    public void StopMusic()
    {
        _audioBg.Stop();
        NotificationManager.Instance.ClearSongNotification();
    }

    private Song GetSong(SongTag title)
    {
        if (_songDict.TryGetValue(title, out Song song))
        {
            return song;
        }
        Debug.LogWarning($"Song not found: {title}");
        return null;
    }

    public void TogglePaused()
    {
        var playing = _audioBg.isPlaying;
        if(playing)
        {
            _audioBg.Pause();
        }
        else
        {
            _audioBg.Play();
        }
    }
    
    private SFX GetSound(SFXTag tag)
    {
        if (_sfxDict.TryGetValue(tag, out SFX sound))
        {
            return sound;
        }
        Debug.LogWarning($"SFX not found: {tag}");
        return null;
    }

    private void Play(AudioClip clip, AudioSource source)
    {
        source.clip = clip;
        source.Play();
    }
    
}
