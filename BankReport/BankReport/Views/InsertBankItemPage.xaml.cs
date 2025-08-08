using BankReport.Interfaces;
using BankReport.Models;
using BankReport.Services.Database;
using BankReport.ViewModels;
using BankReport.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Application = Xamarin.Forms.Application;

using NavigationPage = Xamarin.Forms.NavigationPage;

namespace BankReport.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class InsertBankItemPage : ContentPage
    {
        List<string> simCards;
        private IBankItemService _BankItemService;
        public InsertBankItemPage(IBankItemService BankItemService)
        {
            InitializeComponent();
 

            _BankItemService = BankItemService;
            BindingContext = new InsertBankItemViewModel();
            LoadSimCards();
        }

        private void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            if (e.CurrentSelection.Count > 0)
            {
                var selectedItem = e.CurrentSelection[0] as BankItemModel;
                DisplayAlert("انتخاب شد", $"شما {selectedItem.Name} را انتخاب کردید!", "باشه");
            }
        }

        private void Button_OpenMenu_OnClicked(object sender, EventArgs e)
        {
            menu.OpenMenu(sender,e);
          
        }


        private void LoadSimCards()
        {
            simCards = DependencyService.Get<ISendSms>()?.GetSimCards() ?? new List<string>();
   
        }

  

        private async void ButtonInsertBankItem_OnClicked(object sender, EventArgs e)
        {

            if (string.IsNullOrWhiteSpace(TextBoxBankItemName.Text) || string.IsNullOrWhiteSpace(TextBoxBankItemNumber.Text) )
            {
                DisplayAlert(" خطا", $"فیلد هارا پر کنید", "OK");
                return;
            }

            await _BankItemService.CreateBankItem(new BankItem()
            {
              Id=1,
              CardNumber=TextBoxBankItemNumber.Text,
              Name=TextBoxBankItemName.Text,
                
            });

         //   DependencyService.Get<ISendSms>().SendSmsWithSlot(TextBoxBankItemNumber.Text, $"Admin5={TextBoxMyNumber.Text}", simPicker.SelectedIndex);
            var list = await _BankItemService.GetBankItems();
            await Navigation.PopModalAsync();
      //      await     Navigation.PushModalAsync(new MainPage(_BankItemService));

            Application.Current.MainPage = new NavigationPage(new MainPage(_BankItemService));

        }

        private async void ButtonCansel_Clicked(object sender, EventArgs e)
        {
            await Navigation.PopModalAsync();
            Application.Current.MainPage = new NavigationPage(new MainPage(_BankItemService));
        }

        private void BackButton_Clicked(object sender, EventArgs e)
        {

        }
    }
}