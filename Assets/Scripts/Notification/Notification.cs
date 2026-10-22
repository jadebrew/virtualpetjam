namespace Notification
{
    public enum NotificationType
    {
        Song,
        PetNeedStatus,
        WelcomeBack
    }

    public interface INotification
    {
        public NotificationType Type { get; }
        public NotificationView GetView();
    }
}
