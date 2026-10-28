using UnityEngine;

namespace Notification
{
    public class PetGoesToLitterBoxNotification : INotification
    {
        public NeedType NeedType;

        public NotificationType Type { get; } = NotificationType.RandomEvent;

        public NotificationView GetView()
        {
            var view = ScriptableObject.CreateInstance<NotificationView>();
            view.sprite = NotificationUIManager.Instance.hygieneIcon;
            view.text = $"{Settings.PetName} went to the litterbox.";
            return view;
        }
    }
}
