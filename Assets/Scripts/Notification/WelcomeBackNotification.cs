using System;
using System.Linq;
using Needs;
using UnityEngine;

namespace Notification
{
    public class WelcomeBackNotification : INotification
    {
        public int AmountOfCoinsGained;
        
        public NotificationType Type { get; } = NotificationType.WelcomeBack;
        
        public NotificationView GetView()
        {
            var view = ScriptableObject.CreateInstance<NotificationView>();
            view.sprite = NotificationUIManager.Instance.welcomeBackIcon;

            var coinMessage = AmountOfCoinsGained switch
            {
                0 => "No coins gained today.",
                > 0 => $"While away you earned {AmountOfCoinsGained} ¥.",
                _ => ""
            };

            view.text = $"Welcome back!\n{coinMessage}";
        
            return view;
        }
    }
}
