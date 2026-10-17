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
    
    private Dictionary<SongTag, AudioClip> _songDict = new ();
    private Dictionary<SFXTag, AudioClip> _sfxDict = new ();

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
                _songDict.Add(song.tag, song.audioClip);
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
                _sfxDict.Add(sound.tag, sound.audioClip);
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
        Play(GetSong(tag), _audioBg);
        _audioBg.volume = 0;
        fading = true;
    }
    
    public void Play(SFXTag tag)
    {
        Play(GetSong(tag), _audioSfx);
    }

    public void StopSound()
    {
        _audioSfx.Stop();
    }

    public void StopMusic()
    {
        _audioBg.Stop();
    }

    private AudioClip GetSong(SongTag title)
    {
        if (_songDict.TryGetValue(title, out AudioClip clip))
        {
            return clip;
        }
        Debug.LogWarning($"Song not found: {title}");
        return null;
    }
    
    private AudioClip GetSong(SFXTag tag)
    {
        if (_sfxDict.TryGetValue(tag, out AudioClip clip))
        {
            return clip;
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
