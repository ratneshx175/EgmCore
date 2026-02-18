using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EgmCore.Interfaces
{
    public interface IDeviceManager
    {
        bool IsAckEnabled { get; }
        void SetBillValidatorAck(bool enabled);
    }
}
