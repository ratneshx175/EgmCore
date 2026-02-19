using EgmCore.Services;
using EgmCore.Tests.TestHelpers;
using EgmCore.Tests.TestHelpers;
using Xunit;
using System.Threading.Tasks;

namespace EgmCore.Tests
{
    public class UpdateManagerTests
    {
        [Fact]
        public async Task InstallUpdateAsync_SucceedsForZip()
        {
            var logger = new TestLogger();
            var state = new TestStateManager();
            var um = new UpdateManager(state, logger);

            await um.InstallUpdateAsync("package.zip");

            Assert.Equal("2.0.0-SUCCESS", um.CurrentVersion);
            Assert.Equal("1.0.0", um.LastKnownGood);
            Assert.Equal(EgmCore.Models.EgmState.IDLE, state.CurrentState);
            Assert.Contains(logger.Logs, l => l.Contains("Success") || l.Contains("Success."));
        }

        [Fact]
        public async Task InstallUpdateAsync_FailsForInvalidPackage()
        {
            var logger = new TestLogger();
            var state = new TestStateManager();
            var um = new UpdateManager(state, logger);

            await um.InstallUpdateAsync("badpackage.tar");

            Assert.Equal("1.0.0", um.CurrentVersion);
            Assert.Equal(EgmCore.Models.EgmState.MAINTENANCE, state.CurrentState);
            Assert.Contains(logger.Logs, l => l.Contains("Invalid package format") || l.Contains("Invalid package"));
        }

        [Fact]
        public async Task InstallUpdateAsync_FailsOnPreinstall()
        {
            var logger = new TestLogger();
            var state = new TestStateManager();
            var um = new UpdateManager(state, logger);

            await um.InstallUpdateAsync("fail-package.zip");

            Assert.Equal("1.0.0", um.CurrentVersion);
            Assert.Equal(EgmCore.Models.EgmState.MAINTENANCE, state.CurrentState);
            Assert.Contains(logger.Logs, l => l.Contains("Pre-install script failed") || l.Contains("Pre-install"));
        }
    }
}
