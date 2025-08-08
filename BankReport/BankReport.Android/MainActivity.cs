using Android;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Android.Widget;
using BankReport.Droid.receivers;
using System;

namespace BankReport.Droid
{
    [Activity(Label = "BankReport", Icon = "@mipmap/icon", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize )]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {

        private const int RequestSmsPermissionId = 100;
        // private SmsBroadcastReceiver _smsBroadcastReceiver;
        void CreateNotificationChannel()
        {
            if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
            {
                var channel = new NotificationChannel("sms_channel", "SMS Monitor", NotificationImportance.Default)
                {
                    Description = "Service for monitoring SMS messages"
                };
                var manager = (NotificationManager)GetSystemService(NotificationService);
                manager.CreateNotificationChannel(channel);
            }
        }


        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            //Configuration config = Resources.Configuration;
            //config.FontScale = 1.0f; // جلوگیری از تغییر اندازه فونت
            //CreateConfigurationContext(config);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);
            LoadApplication(new App());

            RequestSmsPermissions();
            IntentFilter filter = new IntentFilter("SMS_RECEIVED_ACTION");

            FFImageLoading.Forms.Platform.CachedImageRenderer.Init(enableFastRenderer: true);
            FFImageLoading.Svg.Forms.SvgCachedImage.Init();

            CreateNotificationChannel();
            var intent = new Intent(this, typeof(MyForegroundService));
            StartForegroundService(intent);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            // پاک کردن داده‌ها
            Xamarin.Essentials.Preferences.Clear();
            // حذف BroadcastReceiver
            //if (_smsBroadcastReceiver != null)
            //{
            //    UnregisterReceiver(_smsBroadcastReceiver);
            //}
        }

        private void RequestSmsPermissions()
        {
            // بررسی نسخه اندروید (تنها برای نسخه‌های 6.0 به بالا نیاز است)
            if ((int)Build.VERSION.SdkInt >= 23)
            {
                // بررسی آیا مجوز داده شده یا نه
                if (CheckSelfPermission(Manifest.Permission.ReceiveSms) != Permission.Granted ||
                CheckSelfPermission(Manifest.Permission.SendSms) != Permission.Granted ||
                CheckSelfPermission(Manifest.Permission.ReadPhoneState) != Permission.Granted ||
                CheckSelfPermission(Manifest.Permission.ReadPhoneNumbers) != Permission.Granted ||
                    CheckSelfPermission(Manifest.Permission.ReadSms) != Permission.Granted)
                {
                    // درخواست مجوز
                    RequestPermissions(new string[]
                    {
                    Manifest.Permission.ReceiveSms,
                    Manifest.Permission.ReadSms,
                    Manifest.Permission.ReadPhoneState,
                    Manifest.Permission.ReadPhoneNumbers,
                    Manifest.Permission.SendSms
                    }, RequestSmsPermissionId);
                }
            }
        }

        // این متد نتیجه درخواست مجوز را دریافت می‌کند
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Permission[] grantResults)
        {
            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            if (requestCode == RequestSmsPermissionId)
            {
                if (grantResults.Length > 0 && grantResults[0] == Permission.Granted)
                {
                    // مجوزها تایید شد
                    Toast.MakeText(this, "مجوز خواندن SMS داده شد.", ToastLength.Short).Show();
                }
                else
                {
                    // مجوز رد شد
                    Toast.MakeText(this, "مجوز خواندن SMS رد شد.", ToastLength.Short).Show();
                }
            }
        }

    }
}