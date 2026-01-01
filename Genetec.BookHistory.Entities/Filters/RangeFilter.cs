using Genetec.BookHistory.Utilities.Extensions;

namespace Genetec.BookHistory.Entities.Filters
{
    public class RangeFilter<T> : IFilter
    {
        public bool IsNegation { get; set; }

        public required T From { get; set; }

        public required T To { get; set; }

        public DateTime GetFromDate()
        {
            return GetDateTimeParameterValue(From);
        }

        public DateTime GetToDate()
        {
            return GetDateTimeParameterValue(To);
        }

        private static DateTime GetDateTimeParameterValue(T value)
        {
            if (value is DateTime dt)
            {
                return dt;
            }
            
            if (value is DateOnly d)
            {
                return d.ConvertToDateTime();
            }

            throw new ArgumentException($"Unsupported type: {typeof(T)}");
        }
    }
}
