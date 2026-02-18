using EgmCore.Interfaces;
using EgmCore.Models;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace EgmCore.Services
{
    public class BillValidatorBackgroundService : BackgroundService
    {
        private readonly IDeviceManager _deviceManager;
        private readonly IEgmStateManager _stateManager;
        private readonly IEgmLogger _logger;

        public BillValidatorBackgroundService(IDeviceManager deviceManager, IEgmStateManager stateManager, IEgmLogger logger)
        {
            _deviceManager = deviceManager;
            _stateManager = stateManager;
            _logger = logger; // Correctly assign to _logger
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using PeriodicTimer timer = new(TimeSpan.FromSeconds(10));
            while (await timer.WaitForNextTickAsync(stoppingToken))
            {
                _logger.Log("Bill validator: Keepalive ping sent");

                // Wait 2 seconds for simulated response
                await Task.Delay(2000, stoppingToken);

                if (!_deviceManager.IsAckEnabled)
                {
                    _logger.Log("ERROR: No ACK received from bill validator — latching fault");
                    if (_stateManager.CurrentState != EgmState.MAINTENANCE)
                        _stateManager.TransitionTo(EgmState.MAINTENANCE);
                }
                else
                {
                    _logger.Log("Bill validator: ACK received");
                }
            }
        }
    }
}
