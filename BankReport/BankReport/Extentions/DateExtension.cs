using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankReport.Extentions
{
    public static class DateExtension
    {
        public static string ToShamsi(this DateTime date)
        {
            var prCalender = new PersianCalendar();
            return prCalender.GetYear(date).ToString("00") + "/"
                + prCalender.GetMonth(date).ToString("00") + "/" + prCalender.GetDayOfMonth(date).ToString("00");
        }
        public static string ToShamsi(this DateTime? date)
        {
            var prCalender = new PersianCalendar();
            return prCalender.GetYear(date.Value).ToString("00") + "/"
                + prCalender.GetMonth(date.Value).ToString("00") + "/" + prCalender.GetDayOfMonth(date.Value).ToString("00");
        }

        public static int GetMonthToShamsi(this DateTime date)
        {
            var prCalender = new PersianCalendar();
            return prCalender.GetMonth(date);
        }
        public static int GetDayToShamsi(this DateTime date)
        {
            var prCalender = new PersianCalendar();
            return prCalender.GetDayOfMonth(date);
        }

        public static int GetYearToShamsi(this DateTime date)
        {
            var prCalender = new PersianCalendar();
            return prCalender.GetYear(date);
        }
        public static DateTime ToMiladi(this string persianDate)
        {
            string[] persianDateText = persianDate.Split('/');

            PersianCalendar persianCalendar = new PersianCalendar();
            var convertedDateResult = new DateTime(int.Parse(persianDateText[0]), int.Parse(persianDateText[1]), int.Parse(persianDateText[2]), persianCalendar);
            return convertedDateResult;
        }

    }
}
