using System;

namespace Assets.Scripts.Extends
{
    public static class TimeExtends
    {
        public static string SHORT_DAY = "g";
        public static string SHORT_HOUR = "s";
        public static string SHORT_MINUTE = "d";
        public static string SHORT_SECONDS = "s";
        public static string GetCountdownText(TimeSpan totalDays)
        {
            string hours = totalDays.Hours >= 10 ? totalDays.Hours.ToString() : "0" + totalDays.Hours;
            string minutes = totalDays.Minutes >= 10 ? totalDays.Minutes.ToString() : "0" + totalDays.Minutes;
            string seconds = totalDays.Seconds >= 10 ? totalDays.Seconds.ToString() : "0" + totalDays.Seconds;

            // Ekrana tarihi basıyoruz.
            if (totalDays.TotalDays >= 1)
                return $"{(int)totalDays.TotalDays}{SHORT_DAY} {hours}{SHORT_HOUR} {minutes}{SHORT_MINUTE} {seconds}{SHORT_SECONDS}";
            if (totalDays.Hours > 0)
                return $"{hours}{SHORT_HOUR} {minutes}{SHORT_MINUTE} {seconds}{SHORT_SECONDS}";
            if (totalDays.Minutes > 0)
                return $"{minutes}{SHORT_MINUTE} {seconds}{SHORT_SECONDS}";
            if (totalDays.Seconds > 0)
                return $"{seconds}{SHORT_SECONDS}";
            return $"0{SHORT_SECONDS}";
        }
    }
}
