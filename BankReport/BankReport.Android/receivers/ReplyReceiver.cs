using Android.Content;
using AndroidX.Core.App;
using BankReport.Models.Temp;
using Xamarin.Forms;

namespace BankReport.Droid.receivers
{

    [BroadcastReceiver(Enabled = true)]
    public class ReplyReceiver : BroadcastReceiver
    {
        public override void OnReceive(Context context, Intent intent)
        {
            var remoteInput = AndroidX.Core.App.RemoteInput.GetResultsFromIntent(intent);
            if (remoteInput != null)
            {
                var message = remoteInput.GetCharSequence("key_text_reply");

                if (!string.IsNullOrWhiteSpace(message))
                { // پاک کردن اعلان مربوطه
                    int notificationId = intent.GetIntExtra("notificationId", -1);
                    if (notificationId != -1)
                    {
                        var notificationManager = NotificationManagerCompat.From(context);
                        //notificationManager.CancelAll();


                        // 🔸 یا نوتیف رو آپدیت کن به حالت "پاسخ داده شد"
                        var repliedNotification = new NotificationCompat.Builder(context, "default")
                            .SetSmallIcon(Android.Resource.Drawable.IcDialogInfo)
                            .SetContentText("پاسخ ارسال شد")
                            .Build();
                        notificationManager.Notify(notificationId, repliedNotification);
                    }
                    Device.BeginInvokeOnMainThread(() =>
                    {
                      //  MessagingCenter.Send<object, string>(this, "ReplyMessage", message);

                        MessagingCenter.Send<object, (string message, int notificationId)>(
                         this,
                         "ReplyMessage",
                         (message, notificationId)
                     );
                    });


                }
            }
        }
    }


}