#!/usr/bin/env bash
set -euo pipefail
current_dir=$(pwd)

_download_dir="/tmp/SpaghettiProjectAppImage/download"
_pkgdir="/tmp/SpaghettiProjectAppImage/AppDir"
_debug_mode=false

readonly PATCHER_NAME="ItalianPatcherByUSPLinux"
readonly DOTNET="dotnet80"
readonly DOTNET_URL="https://builds.dotnet.microsoft.com/dotnet/aspnetcore/Runtime/8.0.19/aspnetcore-runtime-8.0.19-linux-x64.tar.gz"
readonly APPIMAGETOOL_URL="https://github.com/AppImage/appimagetool/releases/download/continuous/appimagetool-x86_64.AppImage"
readonly ICON="$current_dir/SpaghettiCh2/Assets/avalonia-logo.ico"
readonly ICON_INDEX=-1

function debug_echo() {
    if $_debug_mode; then
        echo "${1:-$(cat -)}"
    else
        cat - > /dev/null
    fi
}

function check_command() {
    command -v "$1" >/dev/null 2>&1 || {
        echo "Errore: comando \"$1\" non trovato." >&2
        exit 1
    }
}


function show_help() {
    echo "Usage:	$0 [OPTIONS]"
    echo
    echo "Opzioni:"
    echo "	-k		Conserva i file temporanei alla fine."
    echo "	-d		Mostra output di debug."
    echo "	-h		Mostra questa guida ed esce."
}

keep_temp=false


echo -n "Controllo dipendenze... "
check_command curl
check_command tar
check_command magick
check_command dotnet
echo "OK"

while getopts "df:kch" opt; do
    case $opt in
        k) keep_temp=true ;;
        h) show_help; exit 0 ;;
        d) _debug_mode=true ;;
        *) show_help; exit 1 ;;
    esac
done

# Se $_pkgdir esiste già e non è vuota...
if [ -e "$_pkgdir" ] && [ -n "$(ls -A "$_pkgdir")" ]; then
    echo -n "La directory $_pkgdir non è vuota e sarà necessario eliminarla. Vuoi continuare? (S/n) "
    read -r confirmation
    if [[ "$confirmation" =~ [nN] ]]; then
        echo "Operazione annullata."
        exit 1
    else
        rm -rf "$_pkgdir"
        echo "$_pkgdir è stato eliminato."
    fi
fi


mkdir -p "$_download_dir" "$_pkgdir/usr/bin" "$_pkgdir/opt/spaghettiproject"
cd "$_download_dir"

echo "Scaricamento di $DOTNET..."
curl -L -# "$DOTNET_URL" -o "$DOTNET.tar.gz"

echo "Scaricamento di AppImageTool..."
curl -L -# "$APPIMAGETOOL_URL" -o "appimagetool.AppImage"
chmod +x appimagetool.AppImage

echo -n "Estrazione di dotnet 8.0..."
tar -xzf "$DOTNET.tar.gz" -C "$_pkgdir/usr/bin/"
echo " OK"

cd $current_dir

echo -n "Compilazione del patcher..."
dotnet build -c Release -o $_pkgdir/opt/spaghettiproject
echo " OK"

echo -n "Creazione script di avvio..."
echo "#!/bin/bash

\$APPDIR/usr/bin/dotnet \$APPDIR/opt/spaghettiproject/USPInstaller.dll" > "$_pkgdir/usr/bin/USPInstaller.sh"
chmod +x "$_pkgdir/usr/bin/USPInstaller.sh"
echo " OK"

echo -n "Creazione file .desktop..."
echo "[Desktop Entry]
Type=Application
Name=Undertale Spaghetti Project Installer
Exec=USPInstaller.sh
Icon=uspinstaller
Categories=Utility;" > "$_pkgdir/spaghetti-project.desktop"
echo " OK"

echo "Conversione icona in png..."
magick "$ICON[$ICON_INDEX]" "$_pkgdir/uspinstaller.png"

echo -n "Creazione script AppRun..."
cat << 'EOF' > "$_pkgdir/AppRun"
#!/bin/bash

if [ -z "$APPDIR" ]; then
    APPDIR="$(dirname "$(readlink -f "$0")")"
fi

exec $APPDIR/usr/bin/USPInstaller.sh
EOF
chmod +x "$_pkgdir/AppRun"
echo " OK"

cd "$current_dir"
echo "Creazione AppImage..."
ARCH=x86_64 "$_download_dir/appimagetool.AppImage" "$_pkgdir" "$PATCHER_NAME.AppImage"

if ! $keep_temp; then
    echo -n "Pulizia file temporanei..."
    rm -rf "$_download_dir"
    rm -rf "$_pkgdir"
    echo " OK"
fi

echo "Build completata con successo!"
echo "Il file è: $(pwd)/$PATCHER_NAME.AppImage"
