using System;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using static System.Net.Mime.MediaTypeNames;


namespace BankReport.Services.Bank
{
    public class MelliMessageParser : IMessageParser
    {
        public bool CanParse(string message, string phoneNumber)
        {
            return phoneNumber.Contains("9830009417") || phoneNumber.Contains("+989389114506") || message.Contains("بانك ملي ايران") || message.Contains("حساب:26008");
        }
        #region LastCode
        //        public BankTransaction Parse(string message, string phoneNumber)
        //        {
        //            BankTransaction transaction = new BankTransaction
        //            {
        //                BankName = "Bank Melli",
        //                RawMessage = message,
        //                SenderPhoneNumber = phoneNumber
        //            };

        //            string pattern = @"(?ix)
        //^بانك\s+ملي\s+ايران\s*
        //(?<type>برداشت|انتقال|واریز)[:：]?\s*
        //(?<sign_before>[-+])?\s*
        //(?<amount>[0-9۰-۹٬,]+)\s*
        //(?<sign_after>[-+])?\s*
        //حساب:\s*(?<account>[0-9۰-۹]+)\s*
        //مانده:\s*(?<balance>[0-9۰-۹٬,]+)\s*
        //(?<date>\d{4})-(?<time>\d{1,2}:\d{1,2})"; ;



        //            Match debitMatch = Regex.Match(message.Trim(),
        // pattern);
        //            string sign = debitMatch.Groups["sign_before"].Value + debitMatch.Groups["sign_after"].Value;
        //            if (sign.Contains("-")) sign = "-";
        //            else if (sign.Contains("+")) sign = "+";
        //            else sign = "";
        //            if (debitMatch.Success)
        //            {
        //                transaction.Type = TransactionType.Debit;


        //                transaction.Amount = decimal.Parse(sign + debitMatch.Groups["amount"].Value.Replace(",", ""));
        //                transaction.Balance = decimal.Parse(debitMatch.Groups["balance"].Value.Replace(",", ""));

        //                transaction.AccountId = debitMatch.Groups["account"].Value;

        //                transaction.TransactionDate = DateTime.Now;

        //                return transaction;
        //            }

        //            return null; // If the message format doesn't match
        //        }
        #endregion

        public BankTransaction Parse(string message, string phoneNumber)
        {
            BankTransaction transaction = new BankTransaction
            {
                BankName = "Bank Meli",
                RawMessage = message,
                SenderPhoneNumber = phoneNumber
            };

            string[] lines = message.Split('\n');

            if (lines.Length < 5)
                return null; // فرمت درست نیست

            // خط اول: نام بانک (می‌تونیم نادیده بگیریم)
            string transfer = lines[1].Replace("انتقال:", "").Trim();
            string account = lines[2].Replace("حساب:", "").Trim();
            string balance = lines[3].Replace("مانده:", "").Trim();

            string[] dateTime = lines[4].Split('-');
            string date = dateTime[0].Trim();
            string time = dateTime[1].Trim();

            // بررسی نوع تراکنش از روی علامت آخر مبلغ
            TransactionType type = transfer.Contains("-")
                ? TransactionType.Debit  // برداشت
                : TransactionType.Credit; // واریز

            string sign = "";
            if (lines[1].Contains("-")) sign = "-";
            else if (lines[1].Contains("+")) sign = "+";
            // حذف علامت - یا + برای تبدیل به عدد
            string amountClean =  transfer.Replace("-", "").Replace("+", "").Replace(",", "");
            string balanceClean = balance.Replace(",", "");

            transaction.Type = type;
            transaction.Amount = decimal.Parse(sign+amountClean);
            transaction.Balance = decimal.Parse(sign+balanceClean);
            transaction.AccountId = account;

            // تاریخ/ساعت پیام بانک ملی معمولاً به صورت "MMDD-hh:mm"
            // می‌شه اینجا نگه داشت یا تبدیل کرد
            transaction.TransactionDate = DateTime.Now; // فعلاً زمان فعلی، می‌تونی تبدیل دقیق هم بزنی

            return transaction;
        }


        private DateTime ParseDate(string monthDay, string timeStr)
        {

            int currentYear = DateTime.Now.Year;
            string fullDateStr = $"{currentYear}{monthDay}"; // e.g., "20250502"
            string fullTimeStr = timeStr.Replace(":", ""); // e.g., "1913"

            if (DateTime.TryParseExact($"{fullDateStr}{fullTimeStr}", "yyyyMMddHHmm", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime dt))
            {
                return dt;
            }

      
            return DateTime.MinValue;
        }
    }
}
