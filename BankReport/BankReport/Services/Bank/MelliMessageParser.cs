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
            // Identifies Bank Melli messages based on the sender number or keywords in the message body.
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

            // Regex to match the debit pattern:
            // برداشت:-800,000 حساب:26008 مانده:15,455,820 0502-19:13
            Match debitMatch = Regex.Match(message,
     @"^(?<bank>[\u0600-\u06FF\s]+)\nبرداشت:(?<amount>[\d,]+)-\nحساب:(?<accountId>\d+)\nمانده:(?<balance>[\d,]+)\n(?<date>\d{4})-(?<time>\d{2}:\d{2})$");

            if (debitMatch.Success)
            {
                transaction.Type = TransactionType.Debit;

                // Extract and clean the numeric values.
                transaction.Amount = decimal.Parse(debitMatch.Groups["amount"].Value.Replace(",", ""));
                transaction.Balance = decimal.Parse(debitMatch.Groups["balance"].Value.Replace(",", ""));

                // Extract other details.
                transaction.AccountId = debitMatch.Groups["accountId"].Value;

                // The date format "0502" is ambiguous (MMDD or DDMM). Let's assume MMDD,
                // but this is a point of potential failure if the bank changes the format.
                //string monthDay = debitMatch.Groups["date"].Value;
                //string timeStr = debitMatch.Groups["time"].Value;

                // Use a helper method for date parsing. This is where you would integrate a robust Persian calendar library.
                // For now, this is a placeholder that assumes a Gregorian format with the current year.
                transaction.TransactionDate = DateTime.Now;

                return transaction;
            }

            return null; // If the message format doesn't match
        }

        // A helper method to parse the specific date format for Bank Melli.
        private DateTime ParseDate(string monthDay, string timeStr)
        {
            // Assuming "0502" means May 2nd. The year is not provided, so we'll use the current year.
            // This is a major assumption, and a robust solution requires a Persian calendar library
            // and a way to infer the correct year.
            int currentYear = DateTime.Now.Year;
            string fullDateStr = $"{currentYear}{monthDay}"; // e.g., "20250502"
            string fullTimeStr = timeStr.Replace(":", ""); // e.g., "1913"

            if (DateTime.TryParseExact($"{fullDateStr}{fullTimeStr}", "yyyyMMddHHmm", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime dt))
            {
                return dt;
            }

            // If parsing fails, return a default value.
            return DateTime.MinValue;
        }
    }
}
