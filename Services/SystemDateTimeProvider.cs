namespace AttendanceSystem.Services
{
    public class SystemDateTimeProvider : IDateTimeProvider
    {
        public DateTime Today => DateTime.Now.Date;
    }
}
