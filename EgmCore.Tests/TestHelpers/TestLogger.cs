using EgmCore.Interfaces;
using System.Collections.Concurrent;

namespace EgmCore.Tests.TestHelpers
{
    public class TestLogger : IEgmLogger
    {
        private readonly ConcurrentQueue<string> _logs = new();
        public IReadOnlyCollection<string> Logs => _logs.ToArray();
        public string? TimeZoneSetTo { get; private set; }

        public void Log(string message)
        {
            _logs.Enqueue(message);
        }

        public void SetTimeZone(string timeZoneId)
        {
            TimeZoneSetTo = timeZoneId;
            _logs.Enqueue($"TZ_SET:{timeZoneId}");
        }
    }
}
