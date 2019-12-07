using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Web;

namespace ProjectCSA
{
    public class DateOperations
    {
        public static Random rand = new Random();

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


        public static string ReturnWeeks()
        {

            var exclude = new HashSet<int>() {42, 52, 1, 9, 18, 28, 29, 30, 31, 32, 33};
            IEnumerable<int> range = Enumerable.Range(1, 52).Where(i => !exclude.Contains(i));
            int maxvalue = 52 - exclude.Count;
            int index = rand.Next(0, maxvalue);
            string weekN = range.ElementAt(index).ToString();
            return weekN;
        }

        public static int GetWeeks() { return weeks; }
        public static void SetWeeks(int n) { if (GetWeeks() == 1 && n == 2) { weeks = 53;} if (GetWeeks() == 52 && n == 1) { weeks = 0; } if (n == 2) weeks -= 1; else { weeks += 1; }} // due to technical reasons we had to keep this as one long line otherwise vs wouldnt understand.
    }
}