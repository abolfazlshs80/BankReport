using BankReport.Services.Database;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace BankReport.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class RegisterPasswordCustomAlertPage : ContentPage
    {
        private readonly IUserService _userService;
        private IBankItemService _BankItemService;
        public RegisterPasswordCustomAlertPage(IBankItemService BankItemService, IUserService userService)
        {
            InitializeComponent();
            _BankItemService = BankItemService;
            _userService= userService;
            var User =  _userService.firstUser().Result;
            string fullname = User?.UserName;
            if (!string.IsNullOrWhiteSpace(fullname))
            {
                EntryFullName.Text = fullname;
                EntryFullName.IsEnabled = false;
            }
        }
        private async void OnConfirm(object sender, EventArgs e)
        {
            if (Entrypassword.Text.Equals(Entryrepassword.Text) ||!string.IsNullOrWhiteSpace(Entrypassword.Text) || !string.IsNullOrWhiteSpace(EntryFullName.Text))
            {
                var User =await _userService.firstUser();
                if (User == null)
                {
                    User = new BankReport.Models.Database. User();
                    User.Password = Entrypassword.Text;
                    User.UserName = EntryFullName.Text;
                    await _userService.CreateUser(User);
                }
                else
                {
                    User.Password = Entrypassword.Text;
                    User.UserName = EntryFullName.Text;
                    await _userService.UpdateUser(User);
                }
            
                Application.Current.MainPage = new NavigationPage(new MainPage(_BankItemService));
            }
            else
              await Application.Current.MainPage.DisplayAlert("تایید", "کلمه عبور با هم مطابقت ندارد", "باشه");
        }

        private async void OnCancel(object sender, EventArgs e)
        {
            await Navigation.PopModalAsync(); // بستن دیالوگ
        }
    }
}