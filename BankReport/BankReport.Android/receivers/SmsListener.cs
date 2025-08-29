using Android.App;
using Android.App;
using Android.Content;
using Android.Content;
using Android.OS;
using Android.OS;
using Android.Provider;
using Android.Runtime;
using Android.Telephony;
using Android.Telephony;
using Android.Util;
using Android.Widget;
using BankReport;
using BankReport.Events;
using System;
using System.Linq;

using Android.Content;
using Android.Telephony;
using Android.Util;
using System.Linq;

[BroadcastReceiver(Enabled = true, Exported = true)]
[IntentFilter(new[] { Telephony.Sms.Intents.SmsReceivedAction }, Priority = (int)IntentFilterPriority.HighPriority)]
public class SmsReceiver : BroadcastReceiver
{
    public override void OnReceive(Context context, Intent intent)
    {
        if (intent?.Action != Telephony.Sms.Intents.SmsReceivedAction)
            return;

        try
        {
            var messages = Telephony.Sms.Intents.GetMessagesFromIntent(intent);
            if (messages == null || messages.Length == 0) return;

            // پیام چندبخشی → بهم می‌چسبونیم
            string sender = messages[0].OriginatingAddress;
            string fullBody = string.Concat(messages.Select(m => m.MessageBody));

            Log.Debug("SMSReceiver", $"From: {sender}, Message: {fullBody}");

            // پاس دادن به Forms یا جای دیگه
            App.SMSMessageReceived?.Invoke(sender, fullBody);
        }
        catch (Exception ex)
        {
            Log.Error("SMSReceiver", $"خطا در پردازش SMS: {ex}");
        }
    }
}
