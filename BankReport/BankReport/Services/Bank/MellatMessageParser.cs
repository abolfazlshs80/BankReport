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
                BankName = "Bank Melli",
                RawMessage = message,
                SenderPhoneNumber = phoneNumber
            };
            string pattern = @"(?ix)
^حساب(?<account>[0-9۰-۹]+)\s*
(?<type>برداشت|واریز)[:：]?\s*
(?<sign_before>[-+])?\s*
(?<amount>[0-9۰-۹٬,]+)\s*
(?<sign_after>[-+])?\s*
مانده[:：]?\s*(?<balance>[0-9۰-۹٬,]+)\s*
(?<date>\d{2}\/\d{2}\/\d{2})-(?<time>\d{1,2}:\d{2})";


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

                transaction.AccountId = debitMatch.Groups["accountId"].Value;

                transaction.TransactionDate = DateTime.Now;

                return transaction;
            }

            return null; // If the message format doesn't match
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
