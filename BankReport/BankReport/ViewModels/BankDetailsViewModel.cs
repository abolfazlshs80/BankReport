using BankReport.Models;
using BankReport.Services.Database;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace BankReport.ViewModels
{
    public class BankDetailsViewModel : INotifyPropertyChanged
    {

        public bool StatusBankItemConnection { get; set; }
        public string Type { get; set; }
        public string PhoneNumber { get; set; }
        private ObservableCollection<BankItem> _bankItems;
        public ObservableCollection<BankItem> BankItems
        {
            get => _bankItems;
            set
            {
                if (_bankItems != value)
                {
                    _bankItems = value;
                    SetProperty(ref _bankItems, value);
                }
            }
        }

        public ICommand ToggleCommand => new Command<object>((param) =>
        {
            Console.WriteLine($"پارامتر دریافتی: {param}");
        });
        public Command<BankItem> DeleteBankItemCommand { get; }
        public Command<BankItem> EditBankItemCommand { get; }
        public Command<BankItem> UpdateStatusBankItemCommand { get; }
        private IBankItemService _BankItemService;
        public BankDetailsViewModel(IBankItemService BankItemService)
        {
            _BankItemService = BankItemService;
            BankItems = new ObservableCollection<BankItem>();
            DeleteBankItemCommand = new Command<BankItem>(DeleteAsync);
            EditBankItemCommand = new Command<BankItem>(EditAsync);
            UpdateStatusBankItemCommand = new Command<BankItem>(UpdateStatusBankItem);

            Task a = Task.Run((Action)(async () => await LoadBankItems()));
            a.Wait();
        }


        async void EditAsync(BankItem item)
        {
            //Application.Current.MainPage = new NavigationPage(new UpdateBankItemNew(item.Id, _BankItemService));


            if (item != null)
            {
                await _BankItemService.UpdateBankItem(item);
                await LoadBankItems();
                OnPropertyChanged(nameof(BankItems)); // اطلاع به UI برای به‌روزرسانی
            }

        }

        async void DeleteAsync(BankItem BankItem)
        {
            bool answer = await Application.Current.MainPage.DisplayAlert(
                "تأیید حذف",
                "آیا مطمئن هستید که می‌خواهید این دستگاه را حذف کنید؟",
                "بله", "خیر");

            if (!answer)
            {
                // حذف آیتم
                return;
            }

            var _BankItem = await _BankItemService.FindBankItem(BankItem);
            if (_BankItem != null)
            {
                await _BankItemService.DeleteBankItem(_BankItem);

            }


            if (_BankItem != null)
            {
                BankItems.Remove(_BankItem);
                await LoadBankItems();// حذف دستگاه از ObservableCollection
                await Application.Current.MainPage.DisplayAlert("موفق", "آیتم با موفقیت حذف شد.", "باشه");
            }
        }
        async Task LoadBankItems()
        {
            BankItems = new ObservableCollection<BankItem>(await _BankItemService.GetBankItems());
        }



        public async void UpdateBankItem(BankItem item)
        {
            if (item != null)
            {
                await _BankItemService.UpdateBankItem(item);
                await LoadBankItems();
                OnPropertyChanged(nameof(BankItems)); // اطلاع به UI برای به‌روزرسانی
            }
        }
        public async void UpdateStatusBankItem(BankItem item)
        {

        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)

        {
            PropertyChanged?.Invoke(
                this
                ,
                new
                    PropertyChangedEventArgs(propertyName));
        }
        protected bool SetProperty<T>(ref T backingField, T value, [CallerMemberName] string propertyName = "")
        {
            if (EqualityComparer<T>.Default.Equals(backingField, value))
                return false;

            backingField = value;
            OnPropertyChanged(propertyName);
            return true;
        }

    }



}
