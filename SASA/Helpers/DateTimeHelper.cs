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
    }
}