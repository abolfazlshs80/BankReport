using BankReport.Services;
using System;
using System.Globalization;
using Xamarin.Forms;

namespace BankReport.Converters
{
    public class TransactionTypeToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is TransactionType type)
            {
                switch (type)
                {
                    case TransactionType.Credit:
                        return Color.FromHex("#4CAF50"); // سبز برای واریز
                    case TransactionType.Debit:
                        return Color.FromHex("#F44336");  // قرمز برای برداشت
                    case TransactionType.BalanceInquiry:
                        return Color.FromHex("#2196F3"); // آبی برای استعلام
                    default:
                        return Color.Gray; // رنگ خاکستری برای ناشناخته
                }
            }
            return Color.Default;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
