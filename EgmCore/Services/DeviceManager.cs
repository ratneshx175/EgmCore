using EgmCore.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EgmCore.Services
{
    public class DeviceManager : IDeviceManager
    {
        // Ensure this property matches the interface exactly
        public bool IsAckEnabled { get; private set; } = true;

        private readonly IEgmLogger _logger;

        public DeviceManager(IEgmLogger logger)
        {
            _logger = logger;
        }

        public void SetBillValidatorAck(bool enabled)
        {
            IsAckEnabled = enabled;
            _logger.Log($"[Device] Bill Validator ACK simulation set to: {(enabled ? "ON" : "OFF")}");
        }
    }
}
