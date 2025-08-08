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
                SenderPhoneNumber = phoneNumber // Capture sender for debugging/logging
            };

            // Attempt to parse Debit message
            Match debitMatch = Regex.Match(message, @"حساب\s*(?<accountId>\d+)\s*برداشت\s*(?<amount>[\d,]+)\s*مانده\s*(?<balance>[\d,]+)\s*(?<date>\d{2}/\d{2}/\d{2,4})-(?<time>\d{1,2}:\d{2})");
            if (debitMatch.Success)
            {
                transaction.Type = TransactionType.Debit;
                transaction.Amount = decimal.Parse(debitMatch.Groups["amount"].Value.Replace(",", ""));
                transaction.Balance = decimal.Parse(debitMatch.Groups["balance"].Value.Replace(",", ""));
                transaction.AccountId = debitMatch.Groups["accountId"].Value;
                //  transaction.TransactionDate = ParseDateAndTime(debitMatch.Groups["date"].Value, debitMatch.Groups["time"].Value, "YY/MM/DD");
                transaction.TransactionDate = DateTime.Now;
                return transaction;
            }

            // Attempt to parse Credit (Yaraneh) message
            Match creditYaranehMatch = Regex.Match(message, @"واریز یارانه به حساب\s*(?<accountId>\d+)\s*مبلغ\s*(?<amount>[\d,]+)\s*ریال\s*(?<date>\d{4}/\d{2}/\d{2})");
            if (creditYaranehMatch.Success)
            {
                transaction.Type = TransactionType.Credit;
                transaction.Amount = decimal.Parse(creditYaranehMatch.Groups["amount"].Value.Replace(",", ""));
                transaction.AccountId = creditYaranehMatch.Groups["accountId"].Value;
                transaction.TransactionDate = ParseDateAndTime(creditYaranehMatch.Groups["date"].Value, "00:00", "YYYY/MM/DD"); // Time not present for credit yaraneh example
                                                                                                                                // Balance might not be explicitly stated for credit in this format, or it's the *new* balance. Need to clarify from examples.
                                                                                                                                // For now, let's assume it's not provided in this specific message for balance property.
                return transaction;
            }

            // Attempt to parse Initial Balance message (or just balance update)
            Match balanceMatch = Regex.Match(message, @"مانده\s*(?<balance>[\d,]+)\s*(?<date>\d{2}/\d{2}/\d{2,4})-(?<time>\d{1,2}:\d{2})");
            if (balanceMatch.Success)
            {
                transaction.Type = TransactionType.BalanceInquiry; // Or just a balance update
                transaction.Balance = decimal.Parse(balanceMatch.Groups["balance"].Value.Replace(",", ""));
                transaction.TransactionDate = ParseDateAndTime(balanceMatch.Groups["date"].Value, balanceMatch.Groups["time"].Value, "YY/MM/DD");
                return transaction;
            }

            return null; // If no specific pattern matches
        }

        // Helper method for date parsing (can be moved to a common utility class)
        private DateTime ParseDateAndTime(string dateStr, string timeStr, string dateFormat)
        {
            // Example: "04/04/17" (YY/MM/DD) or "1404/04/26" (YYYY/MM/DD)
            // Example: "11:25"
            string fullDateTimeStr;
            string format;

            if (dateFormat == "YY/MM/DD")
            {
                // Assuming current year is 1404/2025. This needs to be robust for future years.
                // A better approach would be to get the current Persian year and determine the full year.
                int currentPersianYear = 1404; // Assuming current Persian year based on 1404/03/1 in Bank Shahr example.
                string yearPrefix = (currentPersianYear / 100).ToString(); // E.g., "14" for 14xx
                fullDateTimeStr = $"{yearPrefix}{dateStr.Replace("/", "")}{timeStr.Replace(":", "")}";
                format = "yyMMddHHmm"; // Example format
            }
            else if (dateFormat == "YYYY/MM/DD")
            {
                fullDateTimeStr = $"{dateStr.Replace("/", "")}{timeStr.Replace(":", "")}";
                format = "yyyyMMddHHmm";
            }
            else
            {
                // Default or throw exception
                return DateTime.MinValue;
            }

            try
            {
                // Use PersianCalendar for robustness if you have access to it or a similar library.
                // For simplicity here, if the date is gregorian-like (YY/MM/DD), we can try directly parsing.
                // If it's Persian, you'll need a Persian date conversion library.
                // For now, let's assume direct parsing after year adjustment if it's YY format.

                if (dateStr.Length == 8 && dateFormat == "YY/MM/DD") // e.g., "04/04/17"
                {
                    // This is a rough conversion and should be handled by a proper Persian calendar library
                    int year = int.Parse(dateStr.Substring(0, 2)) + 1300; // Assuming 13xx or 14xx
                    if (year < 1400) year += 100; // Simple adjustment for the century

                    string adjustedDateStr = $"{year.ToString().Substring(2, 2)}{dateStr.Substring(3, 2)}{dateStr.Substring(6, 2)}"; // Yymmdd

                    if (DateTime.TryParseExact($"{adjustedDateStr}{timeStr}", "yyMMddHH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime dt))
                    {
                        // This will parse based on Gregorian calendar.
                        // If you need Jalali (Persian) dates, use a library like `PersianCalendar`.
                        return dt;
                    }
                }
                else if (dateStr.Length == 10 && dateFormat == "YYYY/MM/DD") // e.g., "1404/04/26"
                {
                    // You'll need a Persian Calendar library to parse this accurately into a Gregorian DateTime.
                    // For demonstration, let's assume a simplified direct parse if it looks like Gregorian YYYY/MM/DD
                    if (DateTime.TryParseExact($"{dateStr.Replace("/", "")}{timeStr}", "yyyyMMddHHmm", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime dt))
                    {
                        return dt;
                    }
                }

            }
            catch (FormatException)
            {
                // Handle parsing errors
            }
            return DateTime.Now; // Indicate failure
        }
    }



}
