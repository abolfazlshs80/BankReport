
using BankReport.Events;
using BankReport.Interfaces;
using BankReport.Services;
using BankReport.Services.Database;
using BankReport.ViewModels;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;


namespace BankReport.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class BankDetailsPage : ContentPage
    {
        private readonly BankSmsProcessor _smsProcessor;
        private ISendSms _sendSms;
        private IBankTransactionService _bankTransactionService;
     
        public int Id { get; set; }
        public TransactionListViewModel Model { get; set; }
        public BankDetailsPage(IBankTransactionService bankTransactionService, int id)
        {
            Id = id;
            InitializeComponent();
            _bankTransactionService = bankTransactionService;
            this.FlowDirection = FlowDirection.RightToLeft;
            Model = new TransactionListViewModel(bankTransactionService,id);
     
            this.BindingContext = Model;

            // برای تست: شبیه سازی دریافت چند پیامک واقعی پس از بارگذاری صفحه
            

        }


        private async void Button_OpenMenu_OnClicked(object sender, EventArgs e)
        {

            menuBarCustomContent.OpenMenu(sender, e);
        }

        private void BackButton_Clicked(object sender, EventArgs e)
        {
           // GlobalEvents.OnSMSReceived -= null;
            Application.Current.MainPage = new NavigationPage(new MainPage(new BankItemService()));
        }
    }
}