using EgmCore.Models;
using EgmCore.Services;
using EgmCore.Tests.TestHelpers;
using System.Threading.Tasks;
using Xunit;

namespace EgmCore.Tests
{
    public class EgmStateManagerTests
    {
        [Fact]
        public void TransitionTo_ChangesStateAndStartsStopsGameLoop()
        {
            var logger = new TestLogger();
            var sm = new EgmStateManager(logger);

            sm.TransitionTo(EgmState.RUNNING);
            Assert.Equal(EgmState.RUNNING, sm.CurrentState);
            Assert.Contains(logger.Logs, l => l.Contains("Game loop started."));

            sm.TransitionTo(EgmState.IDLE);
            Assert.Equal(EgmState.IDLE, sm.CurrentState);
            Assert.Contains(logger.Logs, l => l.Contains("Stopping game...") || l.Contains("Halted") || l.Contains("Halt"));
        }

        [Fact]
        public void TriggerDoorOpen_TransitionsToMaintenance()
        {
            var logger = new TestLogger();
            var sm = new EgmStateManager(logger);
            sm.TransitionTo(EgmState.RUNNING);

            sm.TriggerDoorOpen();
            Assert.Equal(EgmState.MAINTENANCE, sm.CurrentState);
            Assert.Contains(logger.Logs, l => l.Contains("Door Open Detected") || l.Contains("Maintenance"));
        }
    }
}
