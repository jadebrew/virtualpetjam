using System;
using UnityEngine;

namespace Notification
{
    public class BonusNotification : INotification
    {
        public float Bonus { get; set; } = 1.0f;

        public NotificationType Type { get; } = NotificationType.Song;
        
        public NotificationView GetView()
        {
            var view = ScriptableObject.CreateInstance<NotificationView>();
            view.sprite = NotificationUIManager.Instance.multiplierBonusIcon;
            var bonusString = $"{Bonus:0.##}";// max. two decimal places
            view.text = $"Good job!\nYou get a {bonusString}x Coin Boost.";
        
            return view;
        }
    }
}
