using System;
using System.Collections.Generic;
using System.Text;



using System;
using System.Globalization;
using System.Text.RegularExpressions;


namespace BankReport.Services.Bank
{
    public class MelliMessageParser : IMessageParser
    {
        public bool CanParse(string message, string phoneNumber)
        {
            return phoneNumber.Contains("9830009417") || message.Contains("بانك ملي ايران") || message.Contains("حساب:26008");
        }

        public BankTransaction Parse(string message, string phoneNumber)
        {
            BankTransaction transaction = new BankTransaction
            {
                BankName = "Bank Melli",
                RawMessage = message,
                SenderPhoneNumber = phoneNumber
            };

            string pattern  = @"(?ix)
^بانك\s+ملي\s+ايران\s*
(?<type>برداشت|انتقال|واریز)[:：]?\s*
(?<sign_before>[-+])?\s*
(?<amount>[0-9۰-۹٬,]+)\s*
(?<sign_after>[-+])?\s*
حساب:\s*(?<account>[0-9۰-۹]+)\s*
مانده:\s*(?<balance>[0-9۰-۹٬,]+)\s*
(?<date>\d{4})-(?<time>\d{1,2}:\d{1,2})"; ;



            Match debitMatch = Regex.Match(message.Trim(),
 pattern);
            string sign = debitMatch.Groups["sign_before"].Value + debitMatch.Groups["sign_after"].Value;
            if (sign.Contains("-")) sign = "-";
            else if (sign.Contains("+")) sign = "+";
            else sign = "";
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
