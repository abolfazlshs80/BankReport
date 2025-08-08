using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BankReport.Droid.receivers
{
    [Service(Enabled = true, Exported = true, ForegroundServiceType = ForegroundService.TypeDataSync)]
    public class MyForegroundService : Service
    {
        public const int SERVICE_NOTIFICATION_ID = 1000;

        public override IBinder OnBind(Intent intent) => null;

        public override StartCommandResult OnStartCommand(Intent intent, StartCommandFlags flags, int startId)
        {
            var notification = new Notification.Builder(this, "sms_channel")
                .SetContentTitle("نظارت پیامک بانکی")
                .SetContentText("در حال اجرا برای دریافت پیامک‌ها")
                .SetSmallIcon(Resource.Drawable.icon_about) // آیکون خودت اینجا بذار
                .Build();

            StartForeground(SERVICE_NOTIFICATION_ID, notification);

            // اینجا می‌تونی تایمر یا پردازش دائمی راه بندازی (اختیاری)

            return StartCommandResult.Sticky;
        }
    }

}