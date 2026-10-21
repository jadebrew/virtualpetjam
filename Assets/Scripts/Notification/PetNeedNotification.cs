using UnityEngine;

namespace Notification
{
    public class PetNeedNotification : INotification
    {

        public NotificationView GetView()
        {
            var view = ScriptableObject.CreateInstance<NotificationView>();
            //TODO
            /*
            view.sprite = song.icon;
            view.text = $"Now playing\n{song.artist} - {song.title}";
            */
        
            return view;
        }
    }
}
