using BankReport.Models;
using BankReport.Services;
using BankReport.Services.Database;
using BankReport.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Transactions;
using System.Windows.Input;
using Xamarin.Forms;

namespace BankReport.ViewModels
{
    public class MainBankItemViewModel : INotifyPropertyChanged
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
     
        public MainBankItemViewModel(IBankItemService BankItemService)
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
            Application.Current.MainPage = new NavigationPage(new BankDetailsPage( new BankTransactionService(), item.Id));


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
              await  LoadBankItems();// حذف دستگاه از ObservableCollection
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

    public class CardItemsBankItem
    {
        public string Name { get; set; }
        public string Number { get; set; }
        public bool IsActive { get; set; }
    }
    public class TransactionListViewModel : INotifyPropertyChanged
    {
        IBankTransactionService _bankTransactionService;
        public ObservableCollection<BankTransaction> Transactions { get; set; }



        private ObservableCollection<BankTransactionResult> _bankTransaction;
        public ObservableCollection<BankTransactionResult> BankTransaction
        {
            get => _bankTransaction;
            set
            {
                if (_bankTransaction != value)
                {
                    _bankTransaction = value;
                    OnPropertyChanged(nameof(BankTransaction));
                }
            }
        }
        private BankTransaction _selectedTransaction;
        private bool _isBusy;
        private int  _Id;

        public BankTransaction SelectedTransaction
        {
            get => _selectedTransaction;
            set
            {
                if (_selectedTransaction != value)
                {
                    _selectedTransaction = value;
                    OnPropertyChanged();
                    // می‌توانید اینجا منطقی برای نمایش جزئیات تراکنش انتخاب شده اضافه کنید.
                }
            }
        }

        public bool IsBusy
        {
            get => _isBusy;
            set
            {
                if (_isBusy != value)
                {
                    _isBusy = value;
                    OnPropertyChanged();
                }
            }
        }

        public TransactionListViewModel(IBankTransactionService bankTransactionService,int id)
        {
            _Id = id;
            _bankTransactionService = bankTransactionService;
               Transactions = new ObservableCollection<BankTransaction>();
            BankTransaction=new ObservableCollection<BankTransactionResult>();
            //Transactions = new ObservableCollection<BankTransaction>(bankTransactionService.GetBankTransactions().Result);
       
          // برای تست اولیه

            var results = _bankTransactionService.FindBankTransactionResult(id).Result;
            if (results != null && results.Any())
            {
                CardNumbers = new ObservableCollection<CardItemsBankItem>(
                    results.GroupBy(x => x.CardNumber)
                    .Select(g => g.First())
                    .Select(x => new CardItemsBankItem
                    {
                        Name = x.BankName,
                        Number = x.CardNumber,
                        IsActive = false
                    }).ToList());

                BankTransaction = new ObservableCollection<BankTransactionResult>(
                    results.Where(_ => _.BankName == CardNumbers.FirstOrDefault()?.Name).ToList()
                );
            }
            else
            {
                CardNumbers = new ObservableCollection<CardItemsBankItem>();
                BankTransaction = new ObservableCollection<BankTransactionResult>();
            }

            BankTransaction = new ObservableCollection<BankTransactionResult>(_bankTransactionService.FindBankTransactionResult(id).Result.Where(_ => _.BankName == CardNumbers.FirstOrDefault().Name).ToList());

        }

        // متدی برای افزودن تراکنش جدید (مثلاً وقتی پیامک جدیدی دریافت می‌شود)
        public void AddTransaction(BankTransaction transaction)
        {
            Xamarin.Forms.Device.BeginInvokeOnMainThread(() =>
            {
                Transactions.Insert(0, transaction); // اضافه کردن به ابتدای لیست
            });
        }

        // شبیه‌سازی دریافت پیامک و پردازش آن
        public async Task SimulateSmsReceive(string phoneNumber, string message)
        {
            if (IsBusy) return;
            IsBusy = true;

            // فرض می‌کنیم BankSmsProcessor شما در اینجا قابل دسترسی است.
            // بهترین راه تزریق آن از طریق Dependency Injection است.
            // برای سادگی در این مثال، یک نمونه جدید ایجاد می‌کنیم (در برنامه واقعی این کار را نکنید).
            var smsProcessor = new BankSmsProcessor(); // این را به عنوان یک Singleton یا از طریق DI تزریق کنید!

            await Task.Delay(500); // شبیه‌سازی تأخیر پردازش
            var transaction = smsProcessor.ProcessSms(phoneNumber, message);

            if (transaction != null)
            {
                AddTransaction(transaction);
            }
            else
            {
                Console.WriteLine($"Simulated: Unrecognized SMS from {phoneNumber}: {message}");
            }

            IsBusy = false;
        }


        // پیاده‌سازی INotifyPropertyChanged برای به‌روزرسانی UI
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public ObservableCollection<CardItemsBankItem> CardNumbers { get; set; } = new ObservableCollection<CardItemsBankItem>();

        private CardItemsBankItem _selectedCardNumbers;
        public CardItemsBankItem SelectedCardNumbers
        {
            get => _selectedCardNumbers;
            set
            {
                if (_selectedCardNumbers != value)
                {
                    _selectedCardNumbers = value;
                    OnPropertyChanged(nameof(SelectedCardNumbers));

                    OnCardNumbersChanged();
                }
            }
        }

        private void OnCardNumbersChanged()
        {

            BankTransaction = new ObservableCollection<BankTransactionResult>(_bankTransactionService.FindBankTransactionResult(_Id).Result.Where(_ => _.BankName == CardNumbers.FirstOrDefault().Name &&_.CardNumber== SelectedCardNumbers?.Number).ToList());
            // برای تست اولیه
            // هر کاری که باید بعد از تغییر انتخاب انجام بشه
            Console.WriteLine($"انتخاب شد: {SelectedCardNumbers?.Number}");
        }

        //protected void OnPropertyChanged(string propertyName) =>
        //    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

}


