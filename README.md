# 🎰 EGM Core — .NET 8 Proof-of-Concept

> A lightweight **Electronic Gaming Machine (EGM) Core Simulator** built with **.NET 8**.
> Designed to model state transitions, device communication, update workflows, and audit logging in a clean, testable architecture.

---

## ✨ Overview

**EGM Core** is a modern **.NET 8 console-hosted service** that simulates the internal control layer of an electronic gaming machine.

It provides:

* 🔄 State Machine Management (`IDLE`, `RUNNING`, `UPDATING`, `MAINTENANCE`)
* 🧾 Device Simulation (Bill Validator with ACK heartbeat)
* 📦 Update Manager with rollback capability
* 🌍 OS/Timezone Configuration (simulated `timedatectl`)
* 📝 Structured Audit Logging
* ⏱ Background Keepalive Monitoring (10-second interval)
* 🧩 Clean Architecture with Dependency Injection

> ⚠️ This is a **Proof-of-Concept / Simulation** — no real hardware or OS updates are performed.

---

## 🛠 Built With

* **.NET 8 (LTS)**
* `Microsoft.Extensions.Hosting`
* Dependency Injection
* Background Services (`IHostedService`)
* File-based configuration & logging

---

## 🚀 Getting Started

### Prerequisites

* [.NET 8 SDK](https://dotnet.microsoft.com/download)
* Windows / Linux / macOS

Verify installation:

```bash
dotnet --version
```

Should output `8.x.x`.

---

### Run the Application

From the solution root:

```bash
dotnet run --project EgmCore
```

The application starts:

* Hosted services run in the background
* An interactive CLI is exposed for manual control

---

## 💻 CLI Commands

### ▶ Start a Game

```bash
start_game
```

Transitions system → `RUNNING` and begins simulated game loop.

---

### ⏹ Stop Game

```bash
stop_game
```

Transitions system → `IDLE`.

---

### 🚪 Trigger Door Open

```bash
signal door_open
```

Simulates cabinet door opening → system enters `MAINTENANCE`.

---

### 📦 Install Update

```bash
update package.zip
update --package "my package.zip"
```

Rules:

| Condition            | Result                       |
| -------------------- | ---------------------------- |
| Must end with `.zip` | Accepted                     |
| Contains `"fail"`    | Simulated failure + rollback |
| Success              | Version → `2.0.0-SUCCESS`    |
| Failure              | Reverts to `LastKnownGood`   |

Example failure:

```bash
update update_fail.zip
```

---

### 💳 Bill Validator ACK Control

```bash
device bill_validator ack on
device bill_validator ack off
```

If ACK is disabled:

* Keepalive detects failure
* Fault is latched
* State → `MAINTENANCE`

---

### 🌐 Set Timezone

```bash
os set-timezone UTC
os set-timezone Asia/Kolkata
os set-timezone India
```

Updates:

* `egm_data/system_config.json`
* Logger timestamps immediately switch timezone.

---

### 📊 View Status

```bash
status
```

Displays:

* Current State
* Installed Version

---

## 📁 Data & Logs

| File                          | Purpose                    |
| ----------------------------- | -------------------------- |
| `egm_data/audit.log`          | Full audit trail           |
| `egm_data/system_config.json` | Simulated OS configuration |

Audit timestamps follow the configured timezone (default: **UTC**).

---

## ⚙️ Internal Services

### `EgmStateManager`

Controls the system lifecycle and game loop simulation.

### `DeviceManager`

Maintains simulated device state (ACK enabled/disabled).

### `BillValidatorBackgroundService`

Runs every **10 seconds** to validate device heartbeat.

### `UpdateManager`

Handles:

* Package validation
* Pre-install simulation
* Success / failure workflow
* Automatic rollback logic

### `OsSettingsManager`

Resolves timezone aliases using host OS timezone database.

---

## 🔁 Manual Test Scenario

```bash
dotnet run --project EgmCore
```

Then execute:

```bash
update package.zip
status
```

You should see:

```
State: IDLE
Ver: 2.0.0-SUCCESS
```

Now simulate failure:

```bash
update update_fail.zip
```

System will:

* Log failure
* Roll back to `1.0.0`
* Enter `MAINTENANCE`

---

## 🧱 Project Structure

```
EgmCore/
 ├── Interfaces/          # Contracts
 ├── Services/            # Core implementations
 ├── Background/          # Hosted services
 ├── Managers/            # Domain coordinators
 ├── egm_data/            # Runtime data & logs
 ├── Program.cs           # .NET 8 Host bootstrap
```

---

## 🧩 Extending the System

Add new functionality by:

1. Creating an interface under `Interfaces`
2. Implementing it in `Services`
3. Registering via DI in `Program.cs`

Example:

```csharp
builder.Services.AddSingleton<IMyService, MyService>();
```

---
