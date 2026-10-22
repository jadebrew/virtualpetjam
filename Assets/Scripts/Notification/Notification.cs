namespace Notification
{
    public enum NotificationType
    {
        Song,
        PetNeedStatus
    }

    public interface INotification
    {
        public NotificationType Type { get; }
        public NotificationView GetView();
    }
}
