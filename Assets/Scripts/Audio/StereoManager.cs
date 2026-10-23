using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace Audio
{
    public enum PlaybackState
    {
        Paused,
        Playing
    };
    
    public class StereoManager : MonoBehaviour
    {
        public static StereoManager Instance { get; private set; }
        
        public PlaybackMode PlaybackMode { get; set; } = PlaybackMode.Loop;
        public PlaybackState PlaybackState { get; set; } = PlaybackState.Playing;
        
        private List<Song> _masterPlaylist = new();//full list of all owned songs
        private List<Song> _shufflePool = new();//songs left to play in this shuffle cycle
        private Stack<Song> _playHistory = new();//songs already played this cucle
        
    
        void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
       
        }
        
        public void Pause()
        {
            PlaybackState = PlaybackState.Paused;
            AudioManager.Instance?.PauseSong();
        }
        
        public void Resume()
        {
            PlaybackState = PlaybackState.Playing;
            AudioManager.Instance?.ResumeSong();
        }

        public void PlayNext()
        {
            PlaybackState = PlaybackState.Playing;
            if (_masterPlaylist.Count < 1)
                return;
            if (_masterPlaylist.Count == 1)
            {
                AudioManager.Instance.RestartSong();
                return;
            }
            
            _playHistory.Push(AudioManager.Instance.CurrentSong);
            _shufflePool.RemoveAll(song => song.tag == AudioManager.Instance?.CurrentSong.tag);

            if (_shufflePool.Count == 0)
                RefillPool();

            var newSongTag = _shufflePool.First().tag;
            AudioManager.Instance.Play(newSongTag);
            RemoveCurrentSongFromPool();
        }
        
        public void PlayPrevious()
        {
            PlaybackState = PlaybackState.Playing;
            if (_masterPlaylist.Count < 1)
                return;
            if (_masterPlaylist.Count == 1 || _playHistory.Count < 1)
            {
                AudioManager.Instance.RestartSong();
                return;
            }
            _shufflePool.Add(AudioManager.Instance?.CurrentSong);
            ReshufflePool();
            var newSong = _playHistory.Pop();
            AudioManager.Instance.Play(newSong.tag);
            RemoveCurrentSongFromPool();
        }

        public void ToggleShuffle()
        {
            AudioManager.Instance?.TurnOnShuffle();
            RefillPool();
            ReshufflePool();
        }

        public void ToggleLoop()
        {
            AudioManager.Instance?.TurnOnLoop();
        }

        public void AddSongToPlaylist(SongTag newSongTag)
        {
            if (_masterPlaylist.Any(existingSong => existingSong.tag == newSongTag))
                return;
            
            _masterPlaylist.Add(AudioManager.Instance.GetSong(newSongTag));
            _shufflePool.Add(AudioManager.Instance.GetSong(newSongTag));
            ReshufflePool();
        }

        private void RefillPool()
        {
            _shufflePool = new List<Song>(_masterPlaylist);
            RemoveCurrentSongFromPool();
        }

        private void ReshufflePool()
        {
            ShuffleList(_shufflePool);
        }

        private void RemoveCurrentSongFromPool()
        {
            _shufflePool.RemoveAll(song => song.tag == AudioManager.Instance?.CurrentSong.tag);
        }

        private void ShuffleList<T>(List<T> list)
        {
            // Standard Fisher-Yates shuffle
            for (int i = list.Count - 1; i > 0; i--)
            {
                int j = Random.Range(0, i + 1);
                (list[i], list[j]) = (list[j], list[i]);
            }
        }
    }
}