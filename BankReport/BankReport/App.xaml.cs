using BankReport.Services.Database;
using BankReport.Views;
using Microsoft.Extensions.DependencyInjection;
using Plugin.LocalNotification;
using System;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace BankReport
{
    public partial class App : Application
    {
        public static Action<string, string> SMSMessageReceived;
        public static IServiceProvider ServiceProvider { get; private set; }
        public App()
        {
            InitializeComponent();

            //DependencyService.Register<MockDataStore>();

            var serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection);
            ServiceProvider = serviceCollection.BuildServiceProvider();
            var _deviceService = ServiceProvider.GetRequiredService<IBankItemService>();
            var _userService = ServiceProvider.GetRequiredService<IUserService>();
             _userService.CreateUser1(new Models.Database.User
            {
                UserName = "admin",
                Password = "admin"
            });
             //  MainPage = ServiceProvider.GetRequiredService<MainPage>();
             //   MainPage = new ItemsPage();

             // ذخیره مقدار
             //Preferences.Set("username", "Ali");

             // خواندن مقدار
             //var user = _userService.firstUser().Result;
             //if (user == null)
             //{
             //    MainPage = new NavigationPage(new MainPage(_deviceService));

             //}
             //else
             //{

             //    MainPage = new NavigationPage(new LoginPasswordCustomAlertPage(_deviceService, _userService));

             //}
             MainPage = new NavigationPage(new MainPage(_deviceService));
        }

     

        private void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<MainPage>(); // تزریق صفحه اصلی
            services.AddScoped<DatabaseService>(); // تزریق یک سرویس
  
            services.AddScoped<IBankItemService, BankItemService>(); // تزریق یک سرویس
            services.AddScoped<IBankTransactionService, BankTransactionService>(); // تزریق یک سرویس
            services.AddScoped<IUserService, UserService>(); // تزریق یک سرویس
        }
        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
