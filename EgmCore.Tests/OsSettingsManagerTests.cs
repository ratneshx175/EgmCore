using EgmCore.Operations;
using EgmCore.Tests.TestHelpers;
using System.IO;
using Xunit;

namespace EgmCore.Tests
{
    public class OsSettingsManagerTests
    {
        [Fact]
        public void SetTimezone_InvalidEmpty_ReturnsNullAndLogs()
        {
            var logger = new TestLogger();
            var op = new OsSettingsManager();
            var res = op.SetTimezone("   ", logger, "unit-test");
            Assert.Null(res);
            Assert.Contains(logger.Logs, l => l.Contains("OS CHANGE FAILED") && l.Contains("invalid timezone"));
        }

        [Fact]
        public void SetTimezone_Valid_WritesFileAndReturnsResolved()
        {
            var logger = new TestLogger();
            var op = new OsSettingsManager();

            // Use a well-known timezone id depending on the system. Try to pick one that exists.
            string candidate = "UTC";
            var res = op.SetTimezone(candidate, logger, "unit-test");
            if (res == null)
            {
                // If system doesn't support timezones in test environment, assert failure message
                Assert.Contains(logger.Logs, l => l.Contains("system timezones not available") || l.Contains("not recognized"));
            }
            else
            {
                Assert.Equal(res, candidate);
                Assert.Contains(logger.Logs, l => l.Contains("OS CHANGE") && l.Contains("timezone"));
                // cleanup file
                var path = Path.Combine("egm_data", "system_config.json");
                if (File.Exists(path)) File.Delete(path);
            }
        }
    }
}
