using UnityEngine;

namespace Notification
{
    public class NotificationManager : MonoBehaviour
    {
        public static NotificationManager Instance { get; private set; }
    
        private INotification _currentNotification;//TODO don't really need this
        private int _currentNotificationPrioLevel = 999;
        private NotificationView _currentView;
    
        private PriorityQueue<INotification, int> _notificationQueue = new();


        void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
        }

        public bool ShouldBeReplaced()
        {
            var ( nextUp, nextUpPrioLevel ) = _notificationQueue.PeekWithPriority();
            return (nextUp != null && nextUpPrioLevel <= _currentNotificationPrioLevel);
        }

        public void Next()
        {
            ( _currentNotification, _currentNotificationPrioLevel ) = _notificationQueue.DequeueWithPriority();
            if (_currentNotification == null)
            {
                _currentView = null;
                return;
            }

            _currentView = _currentNotification.GetView();
            
            var ui = NotificationUIManager.Instance;
            ui.ShowNotification(_currentView.text,_currentView.sprite);
        }

        public void NotifySong(Song song)
        {
            _notificationQueue.Enqueue(new SongNotification()
            {
                song = song
            },1);
        }
    
        public void NotifyCatStatus()
        {
            //TODO
        }
    }
}
