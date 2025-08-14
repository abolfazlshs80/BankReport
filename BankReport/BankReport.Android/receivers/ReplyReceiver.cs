using Android.Content;
using AndroidX.Core.App;
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
                {
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        MessagingCenter.Send<object, string>(this, "ReplyMessage", message);
                    });
                }
            }
        }
    }


}