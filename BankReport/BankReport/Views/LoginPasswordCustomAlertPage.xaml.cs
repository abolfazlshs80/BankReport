using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using BankReport.Services.Database;

namespace BankReport.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LoginPasswordCustomAlertPage : ContentPage
    {
        private readonly IBankItemService _BankItemService;
        private readonly IUserService _userService;
        public LoginPasswordCustomAlertPage( IBankItemService BankItemService,IUserService userService)
        {
            InitializeComponent();
            _BankItemService = BankItemService;
            _userService=userService;
        }
        private async void OnConfirm(object sender, EventArgs e)
        {
            var User =await _userService.firstUser();
            if (string.IsNullOrEmpty(EntryPassword.Text))
            {
                await Application.Current.MainPage.DisplayAlert("تایید", "کلمه عبور اشتباه  است", "باشه");
                return;
            }

            string password = User.Password;
            if (EntryPassword.Text.Equals(password))
            {
     
                Application.Current.MainPage = new NavigationPage(new MainPage(_BankItemService));
            }
            else
                await Application.Current.MainPage.DisplayAlert("تایید", "کلمه عبور اشتباه  است", "باشه");
        
     }

        private async void OnCancel(object sender, EventArgs e)
        {
            await Navigation.PopModalAsync(); // بستن دیالوگ
        }
    }
}