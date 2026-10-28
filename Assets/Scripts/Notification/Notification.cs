namespace Notification
{
    public enum NotificationType
    {
        Song,
        PetNeedStatus,
        WelcomeBack,
        CoinReward,
        RandomEvent
    }

    public interface INotification
    {
        public NotificationType Type { get; }
        public NotificationView GetView();
    }
}
