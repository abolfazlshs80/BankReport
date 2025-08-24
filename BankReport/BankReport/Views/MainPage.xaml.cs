
using BankReport.Droid.receivers;
using BankReport.Events;
using BankReport.Interfaces;
using BankReport.Models.Temp;
using BankReport.Services;
using BankReport.Services.Database;
using BankReport.ViewModels;
using Plugin.LocalNotification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
namespace BankReport.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MainPage : ContentPage
    {
        private bool _hasRun = true;
        private List<string> lstMessage;
        private readonly BankSmsProcessor _smsProcessor;
        private ISendSms _sendSms;
        private IBankItemService _BankItemService;
        private IBankTransactionService _BankTransactionService;

        public MainBankItemViewModel Model { get; set; }
        public MainPage(IBankItemService BankItemService)
        {

            InitializeComponent();
            lstMessage = new List<string>();
            //    GlobalEvents.OnSMSReceived += GlobalEvents_OnSMSReceived;
            this.FlowDirection = FlowDirection.RightToLeft;
            _BankItemService = BankItemService;
            BindingContext = Model = new MainBankItemViewModel(_BankItemService);
            _smsProcessor = new BankSmsProcessor();
            _BankTransactionService = new BankTransactionService();

            App.SMSMessageReceived = (sender, message) =>
            {
                Device.BeginInvokeOnMainThread(async () =>
                {

                    BankTransaction transaction = _smsProcessor.ProcessSms(sender, message);
                    Cloner.Value = transaction;
                    DependencyService.Get<Droid.receivers.INotificationService>().ShowMessageWithReply(message);



                });
            };
            MessagingCenter.Subscribe<object, string>(this, "ReplyMessage", async (sender, message) =>
            {
                if (!lstMessage.Contains(message))
                {
                    if (Cloner.Value is BankTransaction transaction)
                    {
                        transaction.Description = message;

                        // اجرای عملیات روی بک‌گراند
                        await Task.Run(() => CreateNewBankTransAction(transaction));
                        Cloner.Value = null;
                    }
                }
                lstMessage.Add(message);
                
            });

            //MessagingCenter.Subscribe<object, string>(this, "ReplyMessage", (sender, message) =>
            //{

            //    if(Cloner.Value!=null&& Cloner.Value is BankTransaction transaction)
            //    {
            //        if(transaction != null)
            //        {

            //            transaction.Description = message;
            //            CreateNewBankTransAction(transaction);
            //        }
            //    }

            //});
        }



        public async void CreateNewBankTransAction(BankTransaction transaction)
        {

            var t = await _BankTransactionService.GetBankTransactions();
            if (transaction != null)
            {
                await _BankTransactionService.CreateBankTransaction(transaction);

                // در اینجا می‌توانید اطلاعات را در پایگاه داده ذخیره کنید،
                // در رابط کاربری نمایش دهید، یا هر کار دیگری که نیاز دارید انجام دهید.
            }
            else
            {
                // پیامک توسط هیچ یک از پارسرهای شناخته شده تشخیص داده نشد یا تجزیه نشد.
                //   await DisplayAlert($"پیامک جدید", "پیامک نامشخص دریافت شد از ", "باشه");
                // می‌توانید این پیامک‌ها را برای بررسی‌های بعدی در یک لاگ جداگانه ذخیره کنید.
            }
        }

        private void GlobalEvents_OnSMSReceived(object sender, SMSEventArgs e)
        {

            //string m = Model.PhoneNumber.Remove(0, 1);
            //if (e.PhoneNumber.Contains("+98" + m))
            //{

            //}


            BankTransaction transaction = _smsProcessor.ProcessSms(e.PhoneNumber, e.Message);

            if (transaction != null)
            {
                // پیامک با موفقیت تجزیه و اطلاعات استخراج شد.
                // حالا می‌توانید این اطلاعات را استفاده کنید:
                Console.WriteLine("تراکنش بانکی جدید:");
                Console.WriteLine($"بانک: {transaction.BankName}");
                Console.WriteLine($"نوع تراکنش: {transaction.Type}");
                Console.WriteLine($"مبلغ: {transaction.Amount}");
                Console.WriteLine($"مانده: {transaction.Balance}");
                Console.WriteLine($"تاریخ و زمان: {transaction.TransactionDate}");
                Console.WriteLine($"شماره کارت/حساب: {(string.IsNullOrEmpty(transaction.CardNumberLastDigits) ? transaction.AccountId : transaction.CardNumberLastDigits)}");
                Console.WriteLine($"توضیحات: {transaction.Description}");
                Console.WriteLine($"پیامک خام: {transaction.RawMessage}");
                Console.WriteLine($"فرستنده: {transaction.SenderPhoneNumber}");

                // در اینجا می‌توانید اطلاعات را در پایگاه داده ذخیره کنید،
                // در رابط کاربری نمایش دهید، یا هر کار دیگری که نیاز دارید انجام دهید.
            }
            else
            {
                // پیامک توسط هیچ یک از پارسرهای شناخته شده تشخیص داده نشد یا تجزیه نشد.
                Console.WriteLine($"پیامک نامشخص دریافت شد از {e.PhoneNumber}: {e.Message}");
                // می‌توانید این پیامک‌ها را برای بررسی‌های بعدی در یک لاگ جداگانه ذخیره کنید.
            }
        }

        private async void Button_OpenMenu_OnClicked(object sender, EventArgs e)
        {

            menuBarCustomContent.OpenMenu(sender, e);
        }


        private async void ButtonInsertBankItem_OnClicked(object sender, EventArgs e)
        {
            var d = await _BankItemService.GetBankItems();
            await Navigation.PushModalAsync(new InsertBankItemPage(_BankItemService));
        }


        public void ShowMessageDialog(string message)
        {
            // ساخت نوتیفیکیشن
            var notification = new NotificationRequest
            {
                NotificationId = 100,
                Title = "پیام جدید",
                Description = message,
                ReturningData = "Dummy data", // داده اختیاری
                                              //Schedule =new NotificationRequestSchedule()
                                              //{
                                              //    NotifyTime = DateTime.Now.AddSeconds(1) // زمان نمایش نوتیفیکیشن
                                              //}
            };

            NotificationCenter.Current.Show(notification);
        }


    }
}