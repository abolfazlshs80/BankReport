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


public class TransactionListViewModel : INotifyPropertyChanged
    {
        IBankTransactionService _bankTransactionService;
        public ObservableCollection<BankTransaction> Transactions { get; set; }
        public BankTransactionResult BankTransaction { get; set; }
        private BankTransaction _selectedTransaction;
        private bool _isBusy;

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
            _bankTransactionService = bankTransactionService;
               Transactions = new ObservableCollection<BankTransaction>();
            BankTransaction=new BankTransactionResult();
            Transactions = new ObservableCollection<BankTransaction>(bankTransactionService.GetBankTransactions().Result);
            BankTransaction = _bankTransactionService.FindBankTransactionResult(id).Result;
            // برای تست اولیه
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

        // داده‌های نمونه برای نمایش اولیه
        private void LoadDummyTransactions()
        {
            Transactions.Add(new BankTransaction
            {
                BankName = "Bank Mellat",
                Type = TransactionType.Debit,
                Amount = 1_500_000m,
                Balance = 2_612_110m,
                TransactionDate = DateTime.Now.AddDays(-1),
                Description = "برداشت با کارت",
                RawMessage = "حساب 1527274495 برداشت 1,500,000 مانده 2,612,110 04/04/28-18:36"
            });

            Transactions.Add(new BankTransaction
            {
                BankName = "Resalat Bank",
                Type = TransactionType.Credit,
                Amount = 1_500_000m,
                Balance = 10_490_490m,
                TransactionDate = DateTime.Now.AddDays(-2),
                Description = "واریز",
                RawMessage = "10.10525354.1 +1,500,000 03/07 20:09 مانده: 10,490,490"
            });

            Transactions.Add(new BankTransaction
            {
                BankName = "Keshavarzi Bank",
                Type = TransactionType.Credit,
                Amount = 5_000_000m,
                Balance = 13_422_960m,
                TransactionDate = DateTime.Now.AddDays(-3),
                Description = "واریز",
                RawMessage = "واریز 5,000,000 مانده 13,422,960 040428-12:44 کارت *0271 bki. ir"
            });

            Transactions.Add(new BankTransaction
            {
                BankName = "Bank Melli",
                Type = TransactionType.Debit,
                Amount = 3_000_000m,
                Balance = 12_455_820m,
                TransactionDate = DateTime.Now.AddDays(-4),
                Description = "برداشت",
                RawMessage = "بانک ملی ایران برداشت: -3,000,000 حساب: 26008 مانده: 12,455,820 0502-20:52"
            });
        }

        // پیاده‌سازی INotifyPropertyChanged برای به‌روزرسانی UI
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

}


