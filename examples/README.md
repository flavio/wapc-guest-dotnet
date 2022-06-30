This directory contains a simple Wasm module that leverages waPC.

The module is defined inside of the `DemoApp` directory, while the
`wapc-host` directory contains a waPC host program that can execute the
WebAssembly module.

## The demo

The WebAssembly module exposes a waPC function called `hello`. This function
takes as input a UTF8 string containing a greeting message.
The function then replies back with a UTF8 encoded string that contains the input
string, the greeting message, followed by the name of the user who is executing
the code.

The WebAssembly module obtains the name of the user that is running the code by
leveraging a waPC function exposed by the host. This function is published by the
host at the "waPC-address" `demo.system.info`. The function takes no input and
returns a Json encoded object.

The Json encoded object has the following structure:

```Json
{
  "realname": "Flavio Castelli",
  "username": "flavio",
  "platform": "linux",
}
```

The WebAssembly module obtains this object, deserializes it and then uses the
`username` value provided by it.

## Requirements

* .NET 7 preview 4 or later must be installed to build the WebAssembly module
* Rust must be [installed](https://rustup.rs/) to compile the `wapc-host` host program

## Compiling

The WebAssembly module can be built with this command:

```console
cd DemoApp
dotnet build
```

The `.wasm` file will be located at `bin/Debug/net7.0/DemoApp.wasm` inside
of the `DemoApp` directory.

The waPC host can be built with this command:

```console
cd wapc-host
cargo build --release
```

## Play time

Once everything is compiled, move into the `wacp-host` directory
and run the following command:

```console
cargo run --release -- ../DemoApp/bin/Debug/net7.0/DemoApp.wasm
```

This will produce an output similar to the following one:

```console
Sending greetings using 'it'...
Ciao flavio!
Sending greetings using 'es'...
Hola flavio!
Sending greetings using 'en'...
Hello flavio!
Sending greetings using 'emoji'...
👋 flavio!
Sending greetings using 'jp'...
こんにちは flavio!
```

The actual username will match the one of the user who is running the `wapc-host`
code.
