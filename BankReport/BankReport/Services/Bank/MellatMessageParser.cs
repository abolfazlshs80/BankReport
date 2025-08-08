using BankReport.Events;
using System;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace BankReport.Services
{


    public class MellatMessageParser : IMessageParser
    {
        public bool CanParse(string message, string phoneNumber)
        {
            // Identify based on sender name or known numbers
            return phoneNumber.Contains("Bank Mellat") || phoneNumber.Contains("1527274495");
        }

        public DateTime ConvertToMiladi(string input)
        {


            // جدا کردن تاریخ و ساعت
            var parts = input.Split('-');
            string datePart = parts[0]; // "04/04/18"
            string timePart = parts[1]; // "20:06"

            // جدا کردن اجزای تاریخ
            var dateParts = datePart.Split('/');
            int day = int.Parse(dateParts[0]);
            int month = int.Parse(dateParts[1]);
            int yearShort = int.Parse(dateParts[2]);

            // سال کامل (مثلاً 1400 + 18 = 1418)
            int baseYear = 1400;  // یا هر پایه که مد نظرته
            int year = baseYear + yearShort;

            // جدا کردن ساعت و دقیقه
            var timeParts = timePart.Split(':');
            int hour = int.Parse(timeParts[0]);
            int minute = int.Parse(timeParts[1]);

            PersianCalendar pc = new PersianCalendar();

            // تبدیل به میلادی
            DateTime gregorianDate = pc.ToDateTime(year, month, day, hour, minute, 0, 0);

            return gregorianDate;
        }
        public BankTransaction Parse(string message, string phoneNumber)
        {
            BankTransaction transaction = new BankTransaction
            {
                BankName = "Bank Mellat",
                RawMessage = message,
                SenderPhoneNumber = phoneNumber
            };
            string pattern = GetPatern();


            Match debitMatch = Regex.Match(message.Trim(),
pattern);
            string sign = debitMatch.Groups["sign_before"].Value + debitMatch.Groups["sign_after"].Value;
            sign = (message.Contains("برداشت")) ? "-" : "+";
            if (sign.Contains("-")) sign = "-";
            else if (sign.Contains("+")) sign = "+";



            if (debitMatch.Success)
            {
                transaction.Type = TransactionType.Debit;


                transaction.Amount = decimal.Parse(sign + debitMatch.Groups["amount"].Value.Replace(",", ""));
                transaction.Balance = decimal.Parse(debitMatch.Groups["balance"].Value.Replace(",", ""));

                transaction.AccountId = debitMatch.Groups["account"].Value;

                transaction.TransactionDate = DateTime.Now;

                return transaction;
            }

            return null; // If the message format doesn't match
        }

        private string GetPatern() => @"(?ix)
^حساب(?<account>[0-9۰-۹]+)\s*
(?<type>برداشت|واریز)[:：]?\s*
(?<sign_before>[-+])?\s*
(?<amount>[0-9۰-۹٬,]+)\s*
(?<sign_after>[-+])?\s*
مانده[:：]?\s*(?<balance>[0-9۰-۹٬,]+)\s*
(?<date>\d{2}\/\d{2}\/\d{2})-(?<time>\d{1,2}:\d{2})";
    }



}
