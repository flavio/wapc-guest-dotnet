use wapc::WapcHost;
use wasmtime_provider::WasmtimeEngineProvider;

mod host_info;
use host_info::HostInfo;

mod greetings;
use greetings::greetings;

fn host_callback(
    _id: u64,
    binding: &str,
    namespace: &str,
    operation: &str,
    _payload: &[u8],
) -> Result<Vec<u8>, Box<dyn std::error::Error + Send + Sync>> {
    match binding {
        "demo" => {
            match namespace {
                "system" => {
                    match operation {
                        "info" => {
                            // there's no payload for this call
                            let host_info = HostInfo::from_env();
                            Ok(serde_json::to_vec(&host_info).expect("Cannot serialize response"))
                        }
                        _ => Err(format!(
                            "unknown operation {} for {}.{}",
                            operation, binding, namespace
                        )
                        .into()),
                    }
                }
                _ => {
                    Err(format!("unknown namespace {} for binding: {}", namespace, binding).into())
                }
            }
        }
        _ => Err(format!("unknown binding: {}", binding).into()),
    }
}

pub fn main() -> Result<(), Box<dyn std::error::Error + Send + Sync>> {
    let args: Vec<String> = std::env::args().collect();
    if args.len() != 2 {
        eprintln!("Wrong usage: you must provide the path to the wasm module to be loaded");
        std::process::exit(1);
    }

    let module_bytes = std::fs::read(&args[1])?;
    let engine = WasmtimeEngineProvider::new(&module_bytes, None)?;
    let host = WapcHost::new(Box::new(engine), Some(Box::new(host_callback)))?;

    for greeting in &greetings() {
        println!("Sending greetings using '{}'...", greeting.lang);
        match host.call("hello", greeting.message.as_bytes()) {
            Ok(r) => {
                let res = String::from_utf8(r)?;
                println!("{}", res);
            }
            Err(e) => {
                eprintln!("error: {}", e);
            }
        }
    }

    Ok(())
}
