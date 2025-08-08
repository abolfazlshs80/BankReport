namespace BankReport.Services
{
    public interface IMessageParser
    {
        // شاید نیاز باشد یک متد برای چک کردن اینکه آیا این پارسر می‌تواند این پیامک را هندل کند هم اضافه کنید.
        bool CanParse(string message, string phoneNumber);
        BankTransaction Parse(string message, string phoneNumber);
    }



}
