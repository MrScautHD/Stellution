echo "Building linux shared"
cargo build --lib --release
echo "Building windows dll"
cargo build --target x86_64-pc-windows-gnu --lib --release