# BibXmr.Client

Async, strongly-typed C#/.NET client for Monero daemon (`monerod`) and wallet (`monero-wallet-rpc`) RPC APIs.

[![NuGet](https://img.shields.io/nuget/v/BibXmr.Client.svg)](https://www.nuget.org/packages/BibXmr.Client)
[![License: MIT](https://img.shields.io/github/license/BibXMR/BibXmr.Client.svg)](https://github.com/BibXMR/BibXmr.Client/blob/main/LICENSE)
[![Frameworks](https://img.shields.io/badge/frameworks-net10.0%20%7C%20net9.0%20%7C%20net8.0-0a66c2)](https://github.com/BibXMR/BibXmr.Client)

> [!CAUTION]
> This library is in alpha. APIs may change and testing is not complete. Use with caution.

## Feature Overview

- Full async RPC coverage for Monero daemon and wallet endpoints.
- Strongly-typed DTOs with source-generated, Native AOT-ready JSON serialization.
- Factory-based client creation with network presets, host/port, or external `HttpClient`.
- Structured exception hierarchy for HTTP, remote RPC, and protocol failures.

## Supported .NET Versions

- .NET 10 (LTS) (`net10.0`)
- .NET 9 (`net9.0`)
- .NET 8 (LTS) (`net8.0`)

> [!NOTE]
> BibXMR NuGets support all .NET releases starting from the oldest currently supported .NET LTS version.

## Installation

```bash
dotnet add package BibXmr.Client
```

## Quickstart

```csharp
using BibXmr.Client.Network;
using BibXmr.Client.Daemon;

// Connect to local mainnet daemon (127.0.0.1:18081)
using var daemon = await MoneroDaemonClient.CreateAsync(MoneroNetwork.Mainnet);

// Calls monerod JSON-RPC method: get_info
var info = await daemon.GetDaemonInformationAsync();

Console.WriteLine($"Version: {info.Version}"); // Version: 0.18.4.6-release
Console.WriteLine($"Height: {info.Height}"); // Height: 3629773
Console.WriteLine($"Restricted RPC: {info.IsRestrictedRpc}"); // Restricted RPC: False
```

## v1.0.0 Roadmap

Coming Soon.

## Contributing

Coming Soon.

## License

Licensed under the [MIT License](https://github.com/BibXMR/BibXmr.Client/blob/main/LICENSE).
