using EgmCore.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EgmCore.Services
{
    public class OSManager : IOSManager
    {
        private string _currentTimezone = "UTC";
        private readonly IEgmLogger _logger;
        private readonly EgmCore.Operations.OsSettingsManager _osSettings;

        public OSManager(IEgmLogger logger, EgmCore.Operations.OsSettingsManager osSettings)
        {
            _logger = logger;
            _osSettings = osSettings;
        }

        public string? SetTimezone(string timezone, string requestedBy)
        {
            var resolved = _osSettings.SetTimezone(timezone, _logger, requestedBy);
            if (resolved != null)
            {
                // update logger timezone
                _logger.SetTimeZone(resolved);
                _currentTimezone = resolved;
                return resolved;
            }

            return null;
        }
    }
}
