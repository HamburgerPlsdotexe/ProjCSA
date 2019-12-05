using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;

namespace ProjectCSA
{
    public class DateOperations
    {
        static int weeks = GetWeekOfYear();
        public static int GetWeekOfYear()
        {
            DateTime time = DateTime.Today;
            DayOfWeek day = CultureInfo.InvariantCulture.Calendar.GetDayOfWeek(time);
            if (day >= DayOfWeek.Monday && day <= DayOfWeek.Wednesday)
            {
                time = time.AddDays(3);
            }
            // Return the week of our adjusted day
            Console.WriteLine(CultureInfo.InvariantCulture.Calendar.GetWeekOfYear(time, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday));
            return CultureInfo.InvariantCulture.Calendar.GetWeekOfYear(time, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);
        }

        public static int GetWeeks() { return weeks; }
        public static void SetWeeks(int n) { if (GetWeeks() == 1 && n == 2) { weeks = 53;} if (GetWeeks() == 52 && n == 1) { weeks = 0; } if (n == 2) weeks -= 1; else { weeks += 1; }} // due to technical reasons we had to keep this as one long line otherwise vs wouldnt understand.
    }
}