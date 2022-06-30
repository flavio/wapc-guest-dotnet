# waPC Guest Library for .NET

> ⚠️ **Warning: experimental** ⚠️

This repository contains a C# implementation of the [waPC](https://wapc.io)
standard for WebAssembly guest modules.
It allows any waPC-compliant WebAssembly host to invoke to procedures inside a
C# compiled guest and similarly for the guest to invoke procedures exposed by the host.

## Requirements

This code leverages [`dot-net-wasi-sdk`](https://github.com/SteveSandersonMS/dotnet-wasi-sdk),
which is currently experimental.

The code requires .NET 7, which is currently (as of July 2022) in preview mode. Executing
`dotnet --version` should return `7.0.100-preview.4` or later.

## Repository layout

The repository contains the following resources:

  * `src`: contains the source code of the waPC guest library
  * `sample`: contains a demo program and a waPC runtime to run it

## Contribute

The code has been tested on a x86_64 Linux machine. It should work out of the
box on other operating systems supported by the .NET platform.

The author of this code is not a .NET expert, patches are welcome to improve the
code quality and to make it more idiomatic.
