using System;

namespace Blog.Core.Brokers.DateTimes
{
    public interface IDateTimeBroker
    {
        DateTimeOffset GetDateTimeOffset();
    }
}