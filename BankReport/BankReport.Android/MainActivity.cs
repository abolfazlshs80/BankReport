using Android;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Widget;
using AndroidX.Core.App;

namespace BankReport.Droid
{
    [Activity(Label = "BankReport", Icon = "@mipmap/icon", Theme = "@style/MainTheme", MainLauncher = true,
        ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        private const int RequestSmsPermissionId = 100;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);
            LoadApplication(new App());

            RequestSmsPermissions();

            // استارت Foreground Service
            var intent = new Intent(this, typeof(Services.MyForegroundService));
            if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
                StartForegroundService(intent);
            else
                StartService(intent);
        }

        private void RequestSmsPermissions()
        {
            if ((int)Build.VERSION.SdkInt >= 23)
            {
                if (CheckSelfPermission(Manifest.Permission.ReceiveSms) != Permission.Granted ||
                    CheckSelfPermission(Manifest.Permission.ReadSms) != Permission.Granted ||
                    CheckSelfPermission(Manifest.Permission.ReadPhoneState) != Permission.Granted ||
                    CheckSelfPermission(Manifest.Permission.ReadPhoneNumbers) != Permission.Granted ||
                    CheckSelfPermission(Manifest.Permission.SendSms) != Permission.Granted ||
                    CheckSelfPermission(Manifest.Permission.PostNotifications) != Permission.Granted)
                {
                    RequestPermissions(new string[]
                    {
                        Manifest.Permission.ReceiveSms,
                        Manifest.Permission.ReadSms,
                        Manifest.Permission.ReadPhoneState,
                        Manifest.Permission.ReadPhoneNumbers,
                        Manifest.Permission.SendSms,
                        Manifest.Permission.PostNotifications
                    }, RequestSmsPermissionId);
                }
            }
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Permission[] grantResults)
        {
            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            if (requestCode == RequestSmsPermissionId)
            {
                if (grantResults.Length > 0 && grantResults[0] == Permission.Granted)
                    Toast.MakeText(this, "مجوز خواندن SMS داده شد.", ToastLength.Short).Show();
                else
                    Toast.MakeText(this, "مجوز خواندن SMS رد شد.", ToastLength.Short).Show();
            }
        }
    }
}
