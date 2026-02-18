using EgmCore.Interfaces;
using EgmCore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EgmCore.Services
{
    public class UpdateManager : IUpdateManager
    {
        private readonly IEgmStateManager _stateManager;
        private readonly IEgmLogger _logger;

        public UpdateManager(IEgmStateManager stateManager, IEgmLogger logger)
        {
            _stateManager = stateManager;
            _logger = logger;
        }

        public string CurrentVersion { get; private set; } = "1.0.0";
        public string LastKnownGood { get; private set; } = "1.0.0";

        public async Task InstallUpdateAsync(string packagePath)
        {
            _stateManager.TransitionTo(EgmState.UPDATING);
            _logger.Log($"[Update] Starting install for: {packagePath}");

            try
            {
                // 1. Validate Manifest
                if (!packagePath.EndsWith(".zip")) throw new Exception("Invalid package format.");

                // 2. Simulate Pre-install Script
                _logger.Log("[Update] Running pre-install scripts...");
                if (packagePath.Contains("fail")) // Simulation trigger
                    throw new Exception("Pre-install script failed with Exit Code 1.");

                await Task.Delay(2000); // Simulate extraction/copying

                // 3. Success
                LastKnownGood = CurrentVersion;
                CurrentVersion = "2.0.0-SUCCESS";
                _logger.Log($"[Update] Success. Current Version: {CurrentVersion}");
                _stateManager.TransitionTo(EgmState.IDLE);
            }
            catch (Exception ex)
            {
                _logger.Log($"[Update Error] {ex.Message}");
                _logger.Log($"[Rollback] Reverting to version: {LastKnownGood}");
                CurrentVersion = LastKnownGood;
                _stateManager.TransitionTo(EgmState.MAINTENANCE);
            }
        }
    }
}
