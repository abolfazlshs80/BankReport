using Android.App;
using Android.Content;
using Android.OS;
using AndroidX.Core.App;

namespace BankReport.Droid.Services
{
    [Service]
    public class MyForegroundService : Service
    {
        public const string CHANNEL_ID = "default";

        public override void OnCreate()
        {
            base.OnCreate();
            CreateNotificationChannel();
        }

        void CreateNotificationChannel()
        {
            if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
            {
                var channel = new NotificationChannel(
                    CHANNEL_ID,
                    "Default Channel",
                    NotificationImportance.Default
                )
                {
                    Description = "Foreground service channel"
                };

                var manager = (NotificationManager)GetSystemService(NotificationService);
                manager.CreateNotificationChannel(channel);
            }
        }

        public override StartCommandResult OnStartCommand(Intent intent, StartCommandFlags flags, int startId)
        {
            var notification = new NotificationCompat.Builder(this, CHANNEL_ID)
                .SetContentTitle("BankReport Service")
                .SetContentText("Service is running in foreground")
                .SetSmallIcon(Android. Resource.Drawable.IcNotificationOverlay) // ⚠️ حتماً داشته باش
                .SetOngoing(true)
                .Build();

            StartForeground(1001, notification); // ⚠️ این خط ضروریست

            // TODO: اینجا می‌تونی کار اصلی سرویس مثل خواندن SMS یا پردازش بانک رو انجام بدی

            return StartCommandResult.Sticky;
        }

        public override IBinder OnBind(Intent intent) => null;
    }
}
