using System;

namespace Assets.Scripts.Extends
{
    public static class TimeExtends
    {
        public static string GetCountdownText(TimeSpan totalDays)
        {
            string hours = totalDays.Hours >= 10 ? totalDays.Hours.ToString() : "0" + totalDays.Hours;
            string minutes = totalDays.Minutes >= 10 ? totalDays.Minutes.ToString() : "0" + totalDays.Minutes;
            string seconds = totalDays.Seconds >= 10 ? totalDays.Seconds.ToString() : "0" + totalDays.Seconds;

            // Ekrana tarihi basıyoruz.
            if (totalDays.TotalDays >= 1)
                return $"{(int)totalDays.TotalDays}{LanguageController.LC.SHORT_DAY} {hours}{LanguageController.LC.SHORT_HOUR} {minutes}{LanguageController.LC.SHORT_MINUTE} {seconds}{LanguageController.LC.SHORT_SECONDS}";
            if (totalDays.Hours > 0)
                return $"{hours}{LanguageController.LC.SHORT_HOUR} {minutes}{LanguageController.LC.SHORT_MINUTE} {seconds}{LanguageController.LC.SHORT_SECONDS}";
            if (totalDays.Minutes > 0)
                return $"{minutes}{LanguageController.LC.SHORT_MINUTE} {seconds}{LanguageController.LC.SHORT_SECONDS}";
            if (totalDays.Seconds > 0)
                return $"{seconds}{LanguageController.LC.SHORT_SECONDS}";
            return $"0{LanguageController.LC.SHORT_SECONDS}";
        }
    }
}
