# waPC Guest Library for .NET

>⚠️ **Warning: experimental** ⚠️
>
> This code leverages [`dot-net-wasi-sdk`](https://github.com/SteveSandersonMS/dotnet-wasi-sdk),
> which is currently marked as experimental.
>
> It also requires usage of .NET 7, which is currently in preview.


This repository contains a C# implementation of the [waPC](https://wapc.io)
standard for WebAssembly guest modules.
It allows any waPC-compliant WebAssembly host to invoke to procedures inside a
C# compiled guest and similarly for the guest to invoke procedures exposed by the host.

The generation of the Wasm module has been tested on a x86_64 Linux machine. It
should work out of the box on other operating systems supported by the .NET
platform.

## Requirements

The code requires .NET 7, which is currently (as of July 2022) in preview mode.
Executing `dotnet --version` should return `7.0.100-preview.4` or later.

## Repository layout

The repository contains the following resources:

  * `src`: contains the source code of the waPC guest library
  * `examples`: contains a demo program and a waPC runtime to run it

## Usage

Create a Console application:

```console
dotnet new console -o MyFirstWapcModule
cd MyFirstWapcModule
dotnet add package Wasi.Sdk --prerelease
dotnet add package WapcGuest
```

Edit the `Program.cs` file and replace its contents to match the following ones:

```cs
using WapcGuest;
using System.Text;

static byte[] callHost(byte[] payload)
{
  var binding = "binding_value";
  var wapcNamespace = "wapc_namespace_value";
  var operation = "operation_value";
  var payloadGuest = "This is the payload coming from the waPC guest";
  var payloadGuestBytes = Encoding.UTF8.GetBytes(payloadGuest);

  var hostResponseBytes = Wapc.HostCall(binding, wapcNamespace, operation, payloadGuestBytes);
  var hostResponse = Encoding.UTF8.GetString(hostResponseBytes);

  string response = $"the host responded with {hostResponse}";
  var responseBytes = Encoding.UTF8.GetBytes(response);

  return responseBytes;
}

static byte[] echo(byte[] payload)
{
  return payload;
}

var wapc = new Wapc();
wapc.RegisterFunction("echo", echo);
wapc.RegisterFunction("callHost", callHost);
```

Finally, build the WebAssembly module in this way:

```console
dotnet build
```

You can now invoke both the `echo` and the `callHost` functions from any
waPC host.

For a more complex example, checkout the `examples` directory.

## Contribute

The author of this code is not a .NET expert, patches are welcome to improve the
code quality and to make it more idiomatic.
