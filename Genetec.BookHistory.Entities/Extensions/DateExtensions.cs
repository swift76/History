namespace Genetec.BookHistory.Entities.Extensions
{
    public static class DateExtensions
    {
        public static DateTime ConvertToDateTime(this DateOnly value)
        {
            return value.ToDateTime(TimeOnly.MinValue);
        }
    }
}
