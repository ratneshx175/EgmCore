using EgmCore.Interfaces;
using EgmCore.Models;
using System.Collections.Generic;

namespace EgmCore.Tests.TestHelpers
{
    public class TestStateManager : IEgmStateManager
    {
        public EgmState CurrentState { get; private set; } = EgmState.IDLE;

        public List<EgmState> Transitions { get; } = new();

        public void TransitionTo(EgmState newState)
        {
            Transitions.Add(newState);
            CurrentState = newState;
        }

        public void TriggerDoorOpen()
        {
            TransitionTo(EgmState.MAINTENANCE);
        }
    }
}
