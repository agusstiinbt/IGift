using IGift.Application.Interfaces.Dates;

namespace IGift.Infrastructure.Services.Dates
{
    public class SystemDateTimeService : IDateTimeService
    {
        public DateTime NowUtc => DateTime.UtcNow;
    }
}
