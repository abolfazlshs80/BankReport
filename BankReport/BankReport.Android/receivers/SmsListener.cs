using System;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Telephony;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Telephony;
using Android.Widget;
using BankReport.Events;
using BankReport;
using Android.Util;

[BroadcastReceiver(Enabled = true, Exported = true)]
[IntentFilter(new[] { "android.provider.Telephony.SMS_RECEIVED" })]
public class SmsReceiver : BroadcastReceiver
{
    public override void OnReceive(Context context, Intent intent)
    {
        if (intent.Action != "android.provider.Telephony.SMS_RECEIVED")
            return;

        var bundle = intent.Extras;
        if (bundle == null) return;

        var pdus = (Java.Lang.Object[])bundle.Get("pdus");
        foreach (var pdu in pdus)
        {
            var format = bundle.GetString("format");
            var smsMessage = SmsMessage.CreateFromPdu((byte[])pdu, format);
            string sender = smsMessage.OriginatingAddress;
            string message = smsMessage.MessageBody;

            Log.Debug("SMSReceiver", $"From: {sender}, Message: {message}");

            // اینجا می‌تونی به لایه Xamarin.Forms پیام بدی:
            App.SMSMessageReceived?.Invoke(sender, message);
        }
    }
}
//namespace SendAndReceiveSMS.Droid.Receivers
//{
//    [BroadcastReceiver(Enabled = true, Exported = true)]
//    [IntentFilter(new[] { "android.provider.Telephony.SMS_RECEIVED" }, Priority = (int)IntentFilterPriority.HighPriority)]
//    public class SmsListener : BroadcastReceiver
//    {
//        protected string message, address = string.Empty;

//        public override void OnReceive(Context context, Intent intent)
//        {
//            if (intent.Action.Equals("android.provider.Telephony.SMS_RECEIVED"))
//            {
//                Bundle bundle = intent.Extras;
//                if (bundle != null)
//                {
//                    try
//                    {
//                        var smsArray = (Java.Lang.Object[])bundle.Get("pdus");

//                        foreach (var item in smsArray)
//                        {
//                            #pragma warning disable CS0618
//                            var sms = SmsMessage.CreateFromPdu((byte[])item);
                           
//                            #pragma warning restore CS0618
//                            address = sms.OriginatingAddress;
//                            message = sms.MessageBody;

//                            GlobalEvents.OnSMSReceived_Event(this, new SMSEventArgs() { PhoneNumber = address, Message = message });

//                        }
//                    }
//                    catch (Exception ex)
//                    {
//                        //Something went wrong.
//                    }
//                }
//            }
//        }
//    }
//}