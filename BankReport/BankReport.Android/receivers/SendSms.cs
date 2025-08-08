using System.Collections.Generic;
using Android.App;
using Android.Content;
using Android.Telephony;
using Android.Widget;
using BankReport.Droid.receivers;
using BankReport.Interfaces;
using Exception = System.Exception;

[assembly: Xamarin.Forms.Dependency(typeof(SendSms))]
namespace BankReport.Droid.receivers
{
    public class SendSms : ISendSms
    {
        public int GetSimCardNumbers()
        {
            int slot = 0;
            var subscriptionManager = (SubscriptionManager)Application.Context.GetSystemService(Context.TelephonySubscriptionService);
            var activeSubscriptionInfoList = subscriptionManager.ActiveSubscriptionInfoList;

            string simNumbers = "";
            if (activeSubscriptionInfoList != null)
            {
                foreach (var subscriptionInfo in activeSubscriptionInfoList)
                {
                    slot++;
                    simNumbers += $"Slot {subscriptionInfo.SimSlotIndex + 1}: {subscriptionInfo.Number}\n";
                }
            }
         //   return simNumbers;
         return slot;
        }
        public string GetSimCardNumberBySlot(int slotIndex)
        {
            var subscriptionManager = (SubscriptionManager)Application.Context.GetSystemService(Context.TelephonySubscriptionService);
            var activeSubscriptionInfoList = subscriptionManager.ActiveSubscriptionInfoList;

            if (activeSubscriptionInfoList != null)
            {
                foreach (var subscriptionInfo in activeSubscriptionInfoList)
                {
                    if (subscriptionInfo.SimSlotIndex == slotIndex)
                    {
                        return subscriptionInfo.Number; // شماره سیم‌کارت این اسلات
                    }
                }
            }
            return null; // اگر شماره‌ای پیدا نشد
        }
        public void Send(string address, string message)
        {
            //var pendingIntent = PendingIntent.GetActivity(Android.App.Application.Context, 0, new Intent(Android.App.Application.Context, typeof(MainActivity)).AddFlags(ActivityFlags.ClearTop | ActivityFlags.NewTask), PendingIntentFlags.NoCreate);

            //SmsManager smsM = SmsManager.Default;
            //smsM.SendTextMessage(address, null, message, pendingIntent, null);

            try
            {
                SmsManager smsManager = SmsManager.Default;

                // ایجاد یک Intent برای ارسال پیام
                Intent sentIntent = new Intent("SMS_SENT");

                // اضافه کردن FLAG_IMMUTABLE برای جلوگیری از خطای API 31 به بالا
                PendingIntent sentPendingIntent = PendingIntent.GetBroadcast(Application.Context, 0, sentIntent, PendingIntentFlags.Immutable);

                smsManager.SendTextMessage(address, null, message, sentPendingIntent, null);

                Toast.MakeText(Application.Context, "پیام ارسال شد", ToastLength.Short).Show();
            }
            catch (Exception ex)
            {
                Toast.MakeText(Application.Context, "خطا در ارسال پیام: " + ex.Message, ToastLength.Long).Show();
            }
        }

        public void SendSmsWithSlot(string number, string message, int simSlot)
        {
            SmsManager smsManager;

            if (Android.OS.Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.LollipopMr1)
            {
                var subscriptionManager = (SubscriptionManager)Application.Context.GetSystemService(Context.TelephonySubscriptionService);
                var subscriptionInfoList = subscriptionManager.ActiveSubscriptionInfoList;
                var subscriptionInfo = subscriptionInfoList[simSlot];
                smsManager = SmsManager.GetSmsManagerForSubscriptionId(subscriptionInfo.SubscriptionId);
            }
            else
            {
                smsManager = SmsManager.Default;
            }

            smsManager.SendTextMessage(number, null, message, null, null);
        }
        public List<string> GetSimCards()
        {
            List<string> simList = new List<string>();

            try
            {
                var context = Application.Context;

                if (Android.OS.Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.LollipopMr1)
                {
                    SubscriptionManager subscriptionManager = (SubscriptionManager)context.GetSystemService(Context.TelephonySubscriptionService);
                    var activeSubscriptionInfoList = subscriptionManager.ActiveSubscriptionInfoList;

                    if (activeSubscriptionInfoList != null)
                    {
                        foreach (var info in activeSubscriptionInfoList)
                        {
                            simList.Add($"{info.DisplayName} ({info.CarrierName}) ({info.Number??""})");
                        }
                    }
                }
                else
                {
                    var telephonyManager = (TelephonyManager)context.GetSystemService(Context.TelephonyService);
                    simList.Add(telephonyManager.NetworkOperatorName);
                }

                if (simList.Count == 0)
                {
                    simList.Add("No SIM detected");
                }
            }
            catch (Exception ex)
            {
                simList.Add($"Error: {ex.Message}");
            }

            return simList;
        }
        public string GetSimOperator()
        {
            try
            {
                var context = Application.Context;
                var telephonyManager = (TelephonyManager)context.GetSystemService(Context.TelephonyService);

                if (telephonyManager != null)
                {
                    return telephonyManager.NetworkOperatorName; // نام اپراتور سیمکارت
                }
            }
            catch (Exception ex)
            {
                return $"Error: {ex.Message}";
            }

            return "Unknown";
        }
        public async void SendAsync(string address, string message)
        {
            SmsManager smsManager = SmsManager.Default;
            smsManager.SendTextMessage(address, null, message, null, null);


        }
    }

}