using UnityEngine;

namespace Notification
{
    public class PetTurnsRadioOffNotification : INotification
    {
        public NeedType NeedType;

        public NotificationType Type { get; } = NotificationType.RandomEvent;

        public NotificationView GetView()
        {
            var view = ScriptableObject.CreateInstance<NotificationView>();
            view.sprite = NotificationUIManager.Instance.sleepIcon;
            view.text = $"{Settings.PetName} turned the radio off.";
            return view;
        }
    }
}
