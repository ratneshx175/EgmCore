using EgmCore.Interfaces;
using EgmCore.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EgmCore.Services
{
    public class EgmStateManager : IEgmStateManager
    {
        private readonly IEgmLogger _logger;
        public EgmStateManager(IEgmLogger logger)
        {
            _logger = logger;
        }
        public EgmState CurrentState { get; private set; } = EgmState.IDLE;
        private CancellationTokenSource? _gameLoopCts;

        public void TransitionTo(EgmState newState)
        {
            if (CurrentState == newState) return;
            _logger.Log($"State Change: {CurrentState} => {newState}");

            if (CurrentState == EgmState.RUNNING)
            {
                _logger.Log("Stopping game...");
                StopGameLoop();
            }

            CurrentState = newState;

            if (newState == EgmState.RUNNING)
            {
                _logger.Log("Game loop started.");
                StartGameLoop();
            }
        }

        public void TriggerDoorOpen()
        {
            _logger.Log("[SIGNAL] Door Open Detected");
            _logger.Log(">> Alert: Entering Maintenance Mode.");
            TransitionTo(EgmState.MAINTENANCE);
        }

        private void StartGameLoop()
        {
            _gameLoopCts = new CancellationTokenSource();
            Task.Run(async () =>
            {
                try
                {
                    while (!_gameLoopCts.Token.IsCancellationRequested)
                    {
                        _logger.Log("[GameThread] Processing RNG and Graphics...");
                        await Task.Delay(3000, _gameLoopCts.Token);
                    }
                }
                catch (OperationCanceledException)
                {
                    _logger.Log("[GameThread] Halted.");
                }
            });
        }

        private void StopGameLoop() => _gameLoopCts?.Cancel();
    }
}
