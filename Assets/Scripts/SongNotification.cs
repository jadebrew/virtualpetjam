using UnityEngine;

public class SongNotification : Notification
{
    public Song song;
        
    public NotificationView GetView()
    {
        var view = ScriptableObject.CreateInstance<NotificationView>();
        view.sprite = song.icon;
        view.text = $"Now playing\n{song.title} - {song.artist}";
        
        return view;
    }
}
