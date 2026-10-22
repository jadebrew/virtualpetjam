using UnityEngine;

namespace Notification
{
    public class CoinRewardNotification : INotification
    {
        
        public NotificationType Type { get; } = NotificationType.CoinReward;
        
        public NotificationView GetView()
        {
            var view = ScriptableObject.CreateInstance<NotificationView>();
            view.sprite = NotificationUIManager.Instance.coinRewardIcon;
            view.text = $"Mimi is very happy.\nYou get a coin!";
        
            return view;
        }
    }
}
