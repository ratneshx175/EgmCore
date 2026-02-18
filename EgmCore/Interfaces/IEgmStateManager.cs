using EgmCore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EgmCore.Interfaces
{
    public interface IEgmStateManager
    {
        EgmState CurrentState { get; }
        void TransitionTo(EgmState newState);
        void TriggerDoorOpen();
    }
}
