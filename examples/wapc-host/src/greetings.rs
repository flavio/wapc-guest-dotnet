pub(crate) struct Greeting {
    pub message: String,
    pub lang: String,
}

pub(crate) fn greetings() -> Vec<Greeting> {
    vec![
        Greeting {
            message: "Ciao".to_string(),
            lang: "it".to_string(),
        },
        Greeting {
            message: "Hola".to_string(),
            lang: "es".to_string(),
        },
        Greeting {
            message: "Hello".to_string(),
            lang: "en".to_string(),
        },
        Greeting {
            message: "๐".to_string(),
            lang: "emoji".to_string(),
        },
        Greeting {
            // gotta trust google translate :)
            message: "ใใใซใกใฏ".to_string(),
            lang: "jp".to_string(),
        },
    ]
}
