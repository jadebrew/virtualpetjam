using UnityEngine;

namespace Notification
{
    public class SongNotification : INotification
    {
        public Song song;
        
        public NotificationView GetView()
        {
            var view = ScriptableObject.CreateInstance<NotificationView>();
            view.sprite = song.icon;
            view.text = $"Now playing\n{song.artist} - {song.title}";
        
            return view;
        }
    }
}
