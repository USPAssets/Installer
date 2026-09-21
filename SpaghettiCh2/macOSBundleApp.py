from pathlib import Path
import subprocess
import shutil

def publish_win_macos(arch: str, is_mac: bool):
    current_path = Path(__file__).parent.resolve()
    proj_path = current_path / "SpaghettiCh2" / "USPInstaller.csproj"
    output_path = current_path / "publish" / arch
    if output_path.exists():
        shutil.rmtree(output_path)

    self_contained_mode = "--no-self-contained"

    # Prepare bundle if we are building mac
    if is_mac:
        plist_path = current_path / "Info.plist"
        icon_path = current_path / "SpaghettiCh2" / "USPInstaller.icns"
        
        app_path = output_path / "USPInstaller.app"
        app_path.mkdir(parents=True)
        
        contents = app_path / "Contents"
        contents.mkdir()

        resources = contents / "Resources"
        resources.mkdir()

        macOSFolder = contents / "MacOS"
        macOSFolder.mkdir()

        shutil.copy2(plist_path, contents)
        shutil.copy2(icon_path, resources)

        output_path = str(macOSFolder)
        self_contained_mode = "--self-contained"

    BUILD_COMMAND = [
        "dotnet", 
        "publish", 
        str(proj_path), 
        '-c', 
        'Release', 
        '-f',
        'net8.0',
        '-r',
        arch, 
        '-p:PublishReadyToRun=false', 
        '-p:PublishTrimmed=false', 
        '-p:DebugType=None',
        '-p:DebugSymbols=false',
        self_contained_mode, 
        '-o',
        output_path
    ]

    subprocess.run(BUILD_COMMAND)

def publish_linux_AppImage(arch: str):
    ENSURE_PUPNET_CMD = [
        "dotnet",
        "tool",
        "update",
        "-g",
        "KuiperZone.PupNet"
    ]
    subprocess.run(ENSURE_PUPNET_CMD, check=True)

    current_path = Path(__file__).parent.resolve()
    pup_file_path = current_path / "USPInstaller.pupnet.conf"
    PUB_APPIMAGE_CMD = [
        "pupnet",
        str(pup_file_path),
        "-r",
        arch,
        "-k",
        "appimage"
    ]
    subprocess.run(PUB_APPIMAGE_CMD)

def main():
    print("""
Cross compilation is currently limited. Check below whether you can compile the target runtime:
- Win: win-x64, win-x86
- Win+WSL: win-x64, win-x86, linux-x64, osx-x64, osx-arm64 (might have to chmod +x the exe)
- macOS: win-x64, win-x86, osx-x64, osx-arm64
- Linux: win-x64, win-x86, linux-x64, osx-x64, osx-arm64 (might have to chmod +x the exe)
    """)

    selection = input("""
Select what to publish and bundle:
(1) win-x64
(2) win-x86 
(3) osx-x64
(4) osx-arm64
(5) linux-x64 \n 
""")
    
    if not selection.isdigit() or int(selection) < 1 or int(selection) > 5:
        raise ValueError("Insert a valid choice to select the architecture.")

    arch = "invalid"
    is_mac = False
    match int(selection):
        case 1:
            arch = "win-x64"
        case 2:
            arch = "win-x86"
        case 3:
            arch = "osx-x64"
            is_mac = True
        case 4:
            arch = "osx-arm64"
            is_mac = True
        case 5:
            arch = "linux-x64"

    if int(selection) < 5:
        publish_win_macos(arch, is_mac)
    else:
        publish_linux_AppImage(arch)

if __name__ == '__main__':
    main()