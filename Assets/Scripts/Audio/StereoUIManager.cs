using UnityEngine;

namespace Audio
{
    public class StereoUIManager : MonoBehaviour
    {
        public static StereoUIManager Instance { get; private set; }
        
        public void OnpauseClicked()
        {
            StereoManager.Instance?.Pause();
        }
        
        public void OnResumeClicked()
        {
            StereoManager.Instance?.Resume();
        }
        
        public void OnNextClicked()
        {
            StereoManager.Instance?.PlayNext();
        }
        
        public void OnPreviousClicked()
        {
            StereoManager.Instance?.PlayPrevious();
        }
        
        public void OnLoopClicked()
        {
            StereoManager.Instance?.ToggleLoop();
        }
        
        public void OnShuffleClicked()
        {
           StereoManager.Instance?.ToggleShuffle();
        }
    
        void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
       
        }
    }
}