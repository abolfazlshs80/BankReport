using Android.App;
using Android.Content;
using AndroidX.Core.App;
using BankReport.Droid;
using BankReport.Droid.receivers;
using BankReport.Models.Temp;
using MyApp.Droid;
using System;
using Xamarin.Forms;

[assembly: Dependency(typeof(NotificationService_Android))]
namespace MyApp.Droid
{
    public class NotificationService_Android : INotificationService
    {
 
        public void ShowMessageWithReply(string message,int notificationId)
        {
            var context = Android.App.Application.Context;

            var replyLabel = "پاسخ خود را وارد کنید";
            var remoteInput = new AndroidX.Core.App.RemoteInput.Builder("key_text_reply")
                .SetLabel(replyLabel)
                .Build();

            // اعلان ID جدید
            int currentId = notificationId;

            // Intent برای BroadcastReceiver
            var replyIntent = new Intent(context, typeof(ReplyReceiver));
            replyIntent.PutExtra("notificationId", currentId); // ID را می‌فرستیم تا بعداً پاک شود

            var replyPendingIntent = PendingIntent.GetBroadcast(
                context,
                currentId, // هر PendingIntent یکتا بشود
                replyIntent,
                PendingIntentFlags.UpdateCurrent | PendingIntentFlags.Mutable
            );

            var action = new NotificationCompat.Action.Builder(
                Android.Resource.Drawable.IcDialogInfo,
                "پاسخ دهید",
                replyPendingIntent
            )
            .AddRemoteInput(remoteInput)
            .Build();

            var builder = new NotificationCompat.Builder(context, "default")
                .SetContentTitle("پیام جدید")
                .SetContentText(message)
                .SetSmallIcon(Android.Resource.Drawable.IcDialogInfo)
                .AddAction(action)
                .SetAutoCancel(true);

            var notificationManager = NotificationManagerCompat.From(context);
            notificationManager.Notify(currentId, builder.Build());
        }
    }
}
