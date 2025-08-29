

namespace BankReport.Droid.receivers
{
    public interface INotificationService
    {
        void ShowMessageWithReply(string message,int notificationId);
    }

}