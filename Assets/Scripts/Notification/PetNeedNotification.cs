using UnityEngine;

namespace Notification
{
    public class PetNeedNotification : INotification
    {
        public NeedType NeedType;

        public NotificationView GetView()
        {
            var view = ScriptableObject.CreateInstance<NotificationView>();

            switch (NeedType)
            {
                case NeedType.Sleep:
                    view.sprite = NotificationUIManager.Instance.sleepIcon;
                    view.text = $"{Settings.PetName} is sleepy!";
                    break;
                case NeedType.Hygiene:
                    view.sprite = NotificationUIManager.Instance.hygieneIcon;
                    view.text = $"{Settings.PetName} is dirty!";
                    break;
                case NeedType.Hunger:
                    view.sprite = NotificationUIManager.Instance.hungerIcon;
                    view.text = $"{Settings.PetName} is hungry!";
                    break;
                default:
                    Debug.LogWarning("NeedType not found");
                    break;
            }
            return view;
        }
    }
}
