namespace B3.Worker.Shared.Utils
{
    public static class DateTimeTools
    {
        public static DateTime SetDateTimeFromTimestamp(long timestamp)
        {
            DateTime dateTime = new(1970, 1, 1, 0, 0, 0, 0);
            dateTime = dateTime.AddSeconds(timestamp);
            return dateTime;
        }
    }
}