using System;
using BankReport.Services.Database;
using Microsoft.Extensions.DependencyInjection;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;


namespace BankReport.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MenuBarCustomContent : Frame
    {
        private IUserService userService;
        public MenuBarCustomContent()
        {
           
            InitializeComponent();
        
             userService = App.ServiceProvider.GetRequiredService<IUserService>();


            var User = userService.firstUser().Result;
            LabelFullName.Text = User.UserName;
        }
        private bool isMenuOpen = false; // وضعیت منو (باز یا بسته)
        public async void OpenMenu(object sender, EventArgs e)
        {
            if (!isMenuOpen)
            {
                MenuPanel.TranslationX = 0;



            }
            else
            {
                MenuPanel.TranslationX = 500;
            }
            isMenuOpen = !isMenuOpen;
        }

        private void ButtonExit_OnClicked(object sender, EventArgs e)
        {
            //DependencyService.Get<ICloseApp>()?.Close();
        }

        private async void ButtonAboutUs_OnClicked(object sender, EventArgs e)
        {
            await Application.Current.MainPage.DisplayAlert("درباره ما",
                "شرکت تولیدی الکترونیک صنعت پیشتازان هوشمند مهر\n" +
                "با افتخار، رسالت خود را ارائه فناوری‌های نوین و به‌روز جهت تضمین امنیت و آسایش شما قرار داده است.\n" +
                "در صورت وجود هرگونه سوال یا پیشنهاد، ما آماده‌ی ارتباط با شما هستیم.\n\n" +

                "راه‌های ارتباطی:\n" +
                "📞 شماره تماس: [۰۹۱۲۸۱۸۸۳۱۱]\n" +
                "📧 ایمیل: [info@smartmehr.ir]\n" +
                "🌐 وب‌سایت: [www.smartmehr.ir]\n" +
                "📸 اینستاگرام: @smartmehr\n\n" +

                "همیشه در خدمت شما هستیم!",
                "باشه");

        }

        private void Buttonhelp_OnClicked(object sender, EventArgs e)
        {

            string guide =
    "راهنمای استفاده از برنامه SMART MEHR\n\n" +
    "صفحه اصلی (صفحه نخست)\n" +
    "- عنوان: SMART MEHR\n" +
    "- جزئیات:\n" +
    "  - در بالای صفحه، نام کاربر (`name`) نمایش داده می‌شود.\n" +
    "  - پنجره‌ای شامل چند آیکون وجود دارد که برای مدیریت دستگاه‌ها و عملیات مختلف استفاده می‌شوند.\n" +
    "  - در پایین صفحه، یک دکمه سبز با علامت '+' قرار دارد که برای افزودن دستگاه جدید استفاده می‌شود.\n\n" +

    "عملیات‌های اصلی در صفحه اول:\n" +
    "1. مشاهده لیست دستگاه‌ها:\n" +
    "   - بر روی نام کاربر کلیک کنید تا به صفحه لیست دستگاه‌ها بروید.\n" +
    "   - در این صفحه، می‌توانید تمام دستگاه‌های ثبت‌شده را مشاهده کنید.\n" +
    "2. افزودن دستگاه جدید:\n" +
    "   - بر روی دکمه '+' در پایین صفحه کلیک کنید.\n" +
    "   - به صفحه اضافه کردن دستگاه جدید منتقل می‌شوید.\n\n" +

    "صفحه اضافه کردن دستگاه جدید\n" +
    "- عنوان: اضافه کردن دستگاه جدید\n" +
    "- جزئیات:\n" +
    "  - در این صفحه، باید اطلاعات دستگاه جدید را وارد کنید.\n" +
    "  - فرم‌ها:\n" +
    "    - نام دستگاه.\n" +
    "    - شماره سیم‌کارت دستگاه.\n" +
    "    - انتخاب نوع دستگاه (با استفاده از آیکون‌ها).\n" +
    "  - دکمه‌ها:\n" +
    "    - افزودن: برای ذخیره اطلاعات دستگاه جدید.\n" +
    "    - انصراف: برای لغو عملیات بدون ذخیره‌سازی.\n\n" +

    "تنظیمات دستگاه\n" +
    "- عنوان: تنظیمات دستگاه\n" +
    "- جزئیات:\n" +
    "  - بعد از انتخاب یک دستگاه، به صفحه تنظیمات دستگاه منتقل می‌شوید.\n" +
    "  - فعال/غیرفعال کردن دستگاه:\n" +
    "    - دو دکمه 'فعال' و 'غیرفعال' برای تغییر وضعیت دستگاه موجود است.\n" +
    "  - تنظیمات دستگاه:\n" +
    "    - دکمه‌هایی برای عملیات‌های مختلف، مانند 'غیرفعال کردن آرام'، 'فعال کردن آرام'، و غیره، وجود دارد.\n" +
    "  - ذخیره تغییرات:\n" +
    "    - برای ذخیره تغییرات، دکمه 'ثبت' را کلیک کنید.\n\n" +

    "تنظیمات کلی\n" +
    "- عنوان: تنظیمات کلی\n" +
    "- جزئیات:\n" +
    "  - این صفحه شامل تنظیمات عمومی برنامه است.\n" +
    "  - تنظیمات ادمین:\n" +
    "    - می‌توانید اطلاعات ادمین‌ها را مدیریت کنید.\n" +
    "    - دکمه‌هایی برای افزودن، ویرایش، یا حذف ادمین‌ها وجود دارد.";

      //      await DisplayAlert("راهنمای استفاده", guide, "باشه");
               Application.Current.MainPage.DisplayAlert("راهنما", guide, "باشه");
            
        }
        private void ButtonChangePassword_OnClicked(object sender, EventArgs e)
        {
            Application.Current.MainPage = new NavigationPage(new RegisterPasswordCustomAlertPage(new BankItemService(),new UserService()));
        }
    }
}