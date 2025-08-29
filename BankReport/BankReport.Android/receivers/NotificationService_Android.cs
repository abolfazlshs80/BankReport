using Android.App;
using Android.Content;
using AndroidX.Core.App;
using Xamarin.Forms;
using MyApp.Droid;
using BankReport.Droid.receivers;
using BankReport.Droid;
using System;

[assembly: Dependency(typeof(NotificationService_Android))]
namespace MyApp.Droid
{
    public class NotificationService_Android : INotificationService
    {
        public void ShowMessageWithReply(string message)
        {
            var context = Android.App.Application.Context;

            var replyLabel = "پاسخ خود را وارد کنید";
            var remoteInput = new AndroidX.Core.App.RemoteInput.Builder("key_text_reply")
                .SetLabel(replyLabel)
                .Build();

            var replyIntent = new Intent(context, typeof(ReplyReceiver));
            var replyPendingIntent = PendingIntent.GetBroadcast(context, 0, replyIntent, PendingIntentFlags.UpdateCurrent | PendingIntentFlags.Mutable);



            var action = new NotificationCompat.Action.Builder(Android.Resource.Drawable.IcDialogInfo, "پاسخ دهید", replyPendingIntent)
                .AddRemoteInput(remoteInput)
                .Build();

            var builder = new NotificationCompat.Builder(context, "default")
                .SetContentTitle("پیام جدید"+new Random().Next(1000))
                .SetContentText(message)
                .SetSmallIcon(Android.Resource.Drawable.IcDialogInfo)
                .AddAction(action)
                .SetAutoCancel(true);

            var notificationId = new Random().Next(10000, 99999);
            var notificationManager = NotificationManagerCompat.From(context);
            notificationManager.Notify(notificationId, builder.Build());
        }
    }
}
