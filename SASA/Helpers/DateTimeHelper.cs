namespace SASA.Helpers
{
    public static class DateTimeHelper
    {
        public static DateTime ToLocalFromUtc(DateTime utcDateTime)
        {
            // Windows (Costa Rica suele ser este)
            try
            {
                var tz = TimeZoneInfo.FindSystemTimeZoneById("Central America Standard Time");
                return TimeZoneInfo.ConvertTimeFromUtc(DateTime.SpecifyKind(utcDateTime, DateTimeKind.Utc), tz);
            }
            catch
            {
                // Linux/macOS (fallback)
                var tz = TimeZoneInfo.FindSystemTimeZoneById("America/Costa_Rica");
                return TimeZoneInfo.ConvertTimeFromUtc(DateTime.SpecifyKind(utcDateTime, DateTimeKind.Utc), tz);
            }
        }

        public static string ToLocalString(DateTime utcDateTime, string format = "yyyy-MM-dd HH:mm:ss")
            => ToLocalFromUtc(utcDateTime).ToString(format);

        public static string FormatearDuracionHoras(int hours)
        {
            if (hours == 168) return "1 semana";
            if (hours == 24) return "1 día";
            if (hours == 4) return "4 horas";
            if (hours == 1) return "1 hora";
            if (hours % 24 == 0) return (hours / 24) + " días";
            return hours + " h";
        }
    }
}