using EgmCore.Services;
using EgmCore.Tests.TestHelpers;
using Xunit;

namespace EgmCore.Tests
{
    public class DeviceManagerTests
    {
        [Fact]
        public void DefaultAckEnabled_IsTrue()
        {
            var logger = new TestLogger();
            var dm = new DeviceManager(logger);
            Assert.True(dm.IsAckEnabled);
        }

        [Fact]
        public void SetBillValidatorAck_UpdatesFlagAndLogs()
        {
            var logger = new TestLogger();
            var dm = new DeviceManager(logger);
            dm.SetBillValidatorAck(false);
            Assert.False(dm.IsAckEnabled);
            Assert.Contains(logger.Logs, l => l.Contains("Bill Validator ACK simulation set to: OFF"));
        }
    }
}
