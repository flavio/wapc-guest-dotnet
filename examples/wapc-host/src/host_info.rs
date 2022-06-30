use serde::Serialize;

#[derive(Serialize, Debug)]
pub(crate) struct HostInfo {
    pub realname: String,
    pub username: String,
    pub platform: String,
}

impl HostInfo {
    pub fn from_env() -> Self {
        HostInfo {
            realname: whoami::realname(),
            username: whoami::username(),
            platform: whoami::platform().to_string(),
        }
    }
}
