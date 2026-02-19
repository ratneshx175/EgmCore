using EgmCore.Interfaces;
using EgmCore.Models;
using EgmCore.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

var builder = Host.CreateApplicationBuilder(args);

// Prevent the host from writing logs to the console (we want only CLI replies on console)
builder.Logging.ClearProviders();


// Register Singletons (Core logic must persist)
builder.Services.AddSingleton<IEgmStateManager, EgmStateManager>();
builder.Services.AddSingleton<IDeviceManager, DeviceManager>();
builder.Services.AddSingleton<IUpdateManager, UpdateManager>();
builder.Services.AddSingleton<IOSManager, OSManager>();
// Register logger
builder.Services.AddSingleton<IEgmLogger, EgmLogger>();
// OS settings helper
builder.Services.AddSingleton<EgmCore.Operations.OsSettingsManager>();

// Register Background Service for 10s Pings
builder.Services.AddHostedService<BillValidatorBackgroundService>();

using IHost host = builder.Build();

// Run CLI Simulation Task
_ = Task.Run(() => RunCli(host.Services));

await host.RunAsync();

static async Task RunCli(IServiceProvider sp)
{
    var state = sp.GetRequiredService<IEgmStateManager>();
    var update = sp.GetRequiredService<IUpdateManager>();
    var device = sp.GetRequiredService<IDeviceManager>();
    var os = sp.GetRequiredService<IOSManager>();
    var logger = sp.GetRequiredService<IEgmLogger>();

    Console.WriteLine("\nEGM CORE MODULE POC LOADED");
    Console.WriteLine("Commands: start_game, stop_game, signal door_open, update --package <name>, device bill_validator ack <on/off>, os set-timezone <tz>, status\n");

    while (true)
    {
        Console.Write("> ");
        var rawInput = Console.ReadLine() ?? "";
        if (string.IsNullOrWhiteSpace(rawInput)) continue;



        // Log the command immediately
        logger.Log($"CMD: {rawInput}");

        var input = rawInput.Split(' ');
        var cmd = input[0].ToLower();

        try
        {
            switch (cmd)
            {
                case "start_game":
                    state.TransitionTo(EgmState.RUNNING);
                    Console.WriteLine($"OK: State -> {state.CurrentState}");
                    break;
                case "stop_game":
                    state.TransitionTo(EgmState.IDLE);
                    Console.WriteLine($"OK: State -> {state.CurrentState}");
                    break;
                case "signal":
                    if (input.Length > 1 && input[1] == "door_open")
                    {
                        state.TriggerDoorOpen();
                        Console.WriteLine("OK: signal processed");
                    }
                    break;
                case "update":
                    {
                        // Support both: `update package.zip` and `update --package package.zip`
                        string? package = null;

                        if (input.Length > 2 && input[1] == "--package")
                        {
                            package = string.Join(' ', input.Skip(2));
                        }
                        else if (input.Length > 1)
                        {
                            package = string.Join(' ', input.Skip(1));
                        }

                        if (!string.IsNullOrWhiteSpace(package))
                        {
                            // Trim optional surrounding quotes
                            package = package.Trim();
                            if (package.StartsWith("\"") && package.EndsWith("\""))
                                package = package[1..^1];

                            Console.WriteLine("OK: update started");
                            await update.InstallUpdateAsync(package);
                        }
                        else
                        {
                            Console.WriteLine("Usage: update --package <name>  or  update <name>");
                        }
                    }
                    break;
                case "device":
                    if (input.Length > 3)
                    {
                        bool on = input[3] == "on";
                        device.SetBillValidatorAck(on);
                        Console.WriteLine($"OK: Bill Validator ACK set to: {(on ? "ON" : "OFF")}");
                    }
                    break;
                case "os":
                    if (input.Length > 2)
                    {
                        var tz = string.Join(' ', input.Skip(2));
                        var resolved = os.SetTimezone(tz, "cli");
                        if (!string.IsNullOrEmpty(resolved))
                            Console.WriteLine($"OK: timezone set to {resolved}");
                        else
                            Console.WriteLine($"Error: timezone '{tz}' not set (see log)");
                    }
                    break;
                case "status":
                    Console.WriteLine($"State: {state.CurrentState} | Ver: {update.CurrentVersion}");
                    break;
            }
        }
        catch (Exception ex)
        {
            logger.Log($"Input Error: {ex.Message}");
            Console.WriteLine($"Error: {ex.Message}");
        }
    }

}

