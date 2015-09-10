using System;

namespace SDC.data.Entity.Audit
{
    public class LogInTrace
    {
        public int Id { get; set; }
        public UserProfile User { get; set; }
        public DateTime Timestamp { get; set; }
        public string IPAddress { get; set; }
    }
}