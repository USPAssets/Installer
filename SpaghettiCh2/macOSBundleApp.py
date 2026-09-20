from pathlib import Path
import subprocess
import shutil

def main():
    selection = input("Bundle x64 (1) or arm64 (2)? \n")
    if not selection.isdigit() or int(selection) < 1 or int(selection) > 2:
        raise ValueError("Insert either 1 or 2 to select the architecture.")

    parsed = int(selection)
    arch = "osx-x64" if parsed == 1 else "osx-arm64"
    current_path = Path(__file__).parent.resolve()
    proj_path = current_path / "SpaghettiCh2" / "USPInstaller.csproj"
    plist_path = current_path / "Info.plist"
    icon_path = current_path / "SpaghettiCh2" / "USPInstaller.icns"
    
    app_path = current_path / "publish" / arch / "USPInstaller.app"
    if app_path.exists():
        shutil.rmtree(app_path)

    app_path.mkdir(parents=True)
    
    contents = app_path / "Contents"
    contents.mkdir()

    resources = contents / "Resources"
    resources.mkdir()

    macOSFolder = contents / "MacOS"
    macOSFolder.mkdir()

    BUILD_COMMAND = [
        "dotnet", 
        "publish", 
        str(proj_path), 
        '-c', 
        'Release', 
        '-r', 
        arch, 
        '-p:PublishReadyToRun=false', 
        '-p:PublishTrimmed=false', 
        '--self-contained', 
        '-o',
        str(macOSFolder)
    ]

    subprocess.run(BUILD_COMMAND)
    shutil.copy2(plist_path, contents)
    shutil.copy2(icon_path, resources)

if __name__ == '__main__':
    main()