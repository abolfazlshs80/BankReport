using BankReport.Services.Bank;
using System.Collections.Generic;

namespace BankReport.Services
{
    public class BankSmsProcessor
    {
        private readonly List<IMessageParser> _parsers;

        public BankSmsProcessor()
        {
            // در اینجا تمام پارسرهای مربوط به هر بانک را اضافه کنید
            _parsers = new List<IMessageParser>
        {
            new MellatMessageParser(),
            new MelliMessageParser(), // اضافه کردن سایر پارسرها
            //new KeshavarziMessageParser(),
            //new ResalatMessageParser(),
            //new BankShahrMessageParser(),
            // ... هر پارسر جدیدی که ایجاد می‌کنید
        };
        }

        public BankTransaction ProcessSms(string phoneNumber, string message)
        {
            foreach (var parser in _parsers)
            {
                if (parser.CanParse(message, phoneNumber))
                {
                    // تلاش برای تجزیه پیامک توسط پارسر مناسب
                    BankTransaction transaction = parser.Parse(message, phoneNumber);
                    if (transaction != null)
                    {
                        return transaction;
                    }
                }
            }
            // اگر هیچ پارسری پیامک را تشخیص نداد یا تجزیه نکرد
            return null;
        }
    }



}
