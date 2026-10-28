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

            if (nextUp == null)
                return false;
            
            if (nextUpPrioLevel < _currentNotificationPrioLevel)
                return true;
            if (nextUpPrioLevel == _currentNotificationPrioLevel)
            {
                return (_currentNotification.Type == nextUp.Type && _currentNotification.Type == NotificationType.Song);
            }

            return false;
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
                Song = song
            },3);
        }
    
        public void NotifyPetStatus(NeedType needType)
        {
            _notificationQueue.Enqueue(new PetNeedNotification()
            {
                NeedType = needType
            },1);
        }
        
        public void NotifyWelcomeBack(int amountOfCoinsGained)
        {
            _notificationQueue.Enqueue(new WelcomeBackNotification()
            {
                AmountOfCoinsGained = amountOfCoinsGained
            },0);
        }
        
        public void NotifyCoinReward()
        {
            _notificationQueue.Enqueue(new CoinRewardNotification(),1);
        }
        
        public void NotifyMultiplyBonus(float multiplyBonus)
        {
            _notificationQueue.Enqueue(new BonusNotification()
            {
                Bonus = multiplyBonus
            },1);
        }
        
        public void NotifyPetTurnedOffRadio()
        {
            _notificationQueue.Enqueue(new PetTurnsRadioOffNotification(), 2);
        }
        
        public void NotifyPetGoesToLitterBox()
        {
            _notificationQueue.Enqueue(new PetGoesToLitterBoxNotification(), 2);
        }
        
        
    }
}
