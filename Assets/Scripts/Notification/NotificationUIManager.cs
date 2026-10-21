using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Notification
{
    public class NotificationUIManager : MonoBehaviour
    {
        public static NotificationUIManager Instance { get; private set; }

        [SerializeField] public GameObject notificationMessage;
        [SerializeField] public TextMeshProUGUI notificationText;
        [SerializeField] public Image notificationImage;
    
        private const float ShowNotificationTimeLength = 10;
        private const float SlideSpeed = 20;
        private const float RootY = 68; //rootY is the Y value notification window will rest when hidden
        private const float ShowY = -16; //showY is the Y value from rootY to slide to
    
        private bool _notificationIsVisible = false; 
        private bool _allowNewNotifications = true;
        private float _currentY;
        private float _timeSinceShown = 0;
    
        private string _lastMsg; // just to check if the notification is new in Show()
    
        void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            
            Instance = this;
            SetNewYPosition(RootY);
        }

        public void ShowNotification(string text, Sprite icon)
        {
            if (!_allowNewNotifications || text == _lastMsg)
                return;
        
            notificationMessage.SetActive(true);
            notificationText.text = text;
            notificationImage.sprite = icon;
            _notificationIsVisible = true;
            _allowNewNotifications = false;
            _lastMsg = text;
            _timeSinceShown = 0;
        }

        private void Update()
        {
            _currentY = notificationMessage.transform.localPosition.y;
            _timeSinceShown += Time.deltaTime;
        
            if (_notificationIsVisible)
            {
                _allowNewNotifications = false;//?? what
            
                if(_currentY > RootY + ShowY)//notification is out of view
                {
                    //slide down
                    _currentY -= SlideSpeed * Time.deltaTime;
                } else {
                    _notificationIsVisible = false;
                }
            }
            else //notification not visible
            {
                if (NotificationManager.Instance.ShouldBeReplaced() ||  _timeSinceShown > ShowNotificationTimeLength) {//shown long enough

                    SlideUp();
                }
            }

            SetNewYPosition(_currentY);
        }

        private void SlideUp()
        {
            _currentY += SlideSpeed * Time.deltaTime;
                
            //stop at root resting spot and hide
            if (_currentY > RootY)
            {
                HideNotification();
                NotificationManager.Instance.Next();
            }
        }

        private void SetNewYPosition(float newY) {
            _currentY = newY;
            notificationMessage.transform.localPosition = new Vector3(
                notificationMessage.transform.localPosition.x,
                newY,
                notificationMessage.transform.localPosition.z
            );
        }
        
        private void HideNotification()
        {
            SetNewYPosition(RootY);
            _allowNewNotifications = true;
            notificationMessage.SetActive(false);
            SetNewYPosition(_currentY);
        }
    }
}