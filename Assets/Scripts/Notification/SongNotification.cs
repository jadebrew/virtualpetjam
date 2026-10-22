using UnityEngine;

namespace Notification
{
    public class SongNotification : INotification
    {
        public Song Song;
        
        public NotificationType Type { get; } = NotificationType.Song;
        
        public NotificationView GetView()
        {
            var view = ScriptableObject.CreateInstance<NotificationView>();
            view.sprite = Song.icon;
            view.text = $"Now playing\n{Song.artist} - {Song.title}";
        
            return view;
        }
    }
}
