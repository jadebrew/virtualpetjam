using System.Collections.Generic;
using UnityEngine;

public class NotificationManager : MonoBehaviour
{
    public static NotificationManager Instance { get; private set; }

    private Notification _highPriorityNotification;
    private Notification _songNotification;
    private NotificationView _currentView;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public void Tick()
    {
        if (_highPriorityNotification != null)
        {
            _currentView = _highPriorityNotification.GetView();
        }
        else if (_songNotification != null)
        {
            _currentView = _songNotification.GetView();
        }
        else
        {
            _currentView = null;
        }

        var ui = UIManager.Instance;
        if (!_currentView)
        {
            ui.notificationMessage.SetActive(false);
            return;
        }

        if (ui.isIdle())
        {
            ui.notificationText.text = _currentView.text;
            ui.NotificationImage.sprite = _currentView.sprite;
            ui.notificationMessage.SetActive(true);
            ui.Show();
        }
    }

    public void NotifySong(Song song)
    {
        _songNotification = new SongNotification()
        {
            song = song
        };
    }

    public void ClearSongNotification()
    {
        _songNotification = null;
    }
}
