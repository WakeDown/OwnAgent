using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Helpers
{
    public class TimeHelper
    {
        const int SECOND = 1;
        const int MINUTE = 60 * SECOND;
        const int HOUR = 60 * MINUTE;
        const int DAY = 24 * HOUR;
        const int MONTH = 30 * DAY;



        public static string CalculateTimeAgo(DateTime current, DateTime target)
        {
            var ts = new TimeSpan(current.Ticks - target.Ticks);
            double delta = Math.Abs(ts.TotalSeconds);

            if (delta < 1 * MINUTE)
                return ts.Seconds == 1 ? "секунду назад" : ts.Seconds + " секунд назад";

            if (delta < 2 * MINUTE)
                return "минуту назад";

            if (delta < 45 * MINUTE)
                return ts.Minutes + " минут назад";

            if (delta < 90 * MINUTE)
                return "час назад";

            if (delta < 24 * HOUR)
                return ts.Hours + " часов назад";

            if (delta < 48 * HOUR)
                return "вчера";

            if (delta < 30 * DAY)
                return ts.Days + " дней назад";

            if (delta < 12 * MONTH)
            {
                int months = Convert.ToInt32(Math.Floor((double)ts.Days / 30));
                return months <= 1 ? "месяц назад" : months + " месяцев назад";
            }
            else
            {
                int years = Convert.ToInt32(Math.Floor((double)ts.Days / 365));
                return years <= 1 ? "год назад" : years + " лет назад";
            }
        }
    }
}
