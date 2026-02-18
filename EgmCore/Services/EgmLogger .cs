using EgmCore.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EgmCore.Services
{
    public class EgmLogger : IEgmLogger
    {
        private TimeZoneInfo _currentTimeZone = TimeZoneInfo.Local;
        private readonly string _logFilePath = "egm_data/audit.log";
        private readonly object _lock = new();

        public EgmLogger()
        {
            if (!Directory.Exists("egm_data")) Directory.CreateDirectory("egm_data");
        }

        public void SetTimeZone(string timeZoneId)
        {
            try
            {
                _currentTimeZone = TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
            }
            catch
            {
                _currentTimeZone = TimeZoneInfo.Utc;
            }
        }

        public void Log(string message)
        {
            DateTime localTime = TimeZoneInfo.ConvertTime(DateTime.Now, _currentTimeZone);
            // Convert from UTC to the configured timezone using DateTimeOffset to preserve the correct offset
            var offsetTime = TimeZoneInfo.ConvertTime(DateTimeOffset.UtcNow, _currentTimeZone);
            string timestamp = offsetTime.ToString("yyyy-MM-ddTHH:mm:ss.fffffffzzz");
            string formattedLog = $"{timestamp} | {message}";

            lock (_lock)
            {
                File.AppendAllText(_logFilePath, formattedLog + Environment.NewLine);
            }
        }
    }
}
