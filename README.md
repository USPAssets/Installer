
# Installer universale per DELTARUNE e UNDERTALE by USP

Ciao! 
Finalmente abbiamo un installer universale! Con questo potrete patchare *UNDERTALE* e *DELTARUNE* (Capitolo 1 e 2), sia su **Windows**, **macOS** e **Linux**! L'installer vi permettera' anche di aggiornare la patch nel caso rilasceremo aggiornamenti. Insomma, conveniente!

## Compatibilita'

L'installer e' compatibile con le seguenti versioni dei giochi:
- **DELTARUNE (Chaper 1 & 2 DEMO)**
- **DELTARUNE**, testato con:
	- Capitolo 1 v1.38
	- Capitolo 2 v1.44
- **UNDERTALE**, v1.08

E con qualsiasi versione di Windows, macOS o distribuzione Linux in grado di far girare **.NET Runtime 8.0** (piu' info nei prerequisit).

## Prerequisiti

- Se avete installato la patch in precedenza, reinstallate il gioco seguendo le istruzioni nella [sezione qui sotto](#rimuovere-la-patch).
- Come prima cosa, trovate la cartella dove avete installato *UNDERTALE* o *DELTARUNE*; generalmente, per esempio su Windows, se avete installato il gioco tramite Steam, questa sarà:
```
// Per Undertale
C:\Programmi(x86)\Steam\steamapps\common\Undertale 
// Per DELTARUNE Demo
C:\Programmi(x86)\Steam\steamapps\common\DELTARUNEdemo
// Per DELTARUNE
C:\Programmi(x86)\Steam\steamapps\common\DELTARUNE
```
- Installate il **runtime di .NET 8.0**:
	- **Windows**: Andate sulla [pagina ufficiale](https://dotnet.microsoft.com/download/dotnet/8.0/runtime) e selezionate *Download x64* o *Download x86* sotto *Run desktop apps*, a seconda se avete un sistema a 64 o 32 bit rispettivamente, e installate il runtime seguendo le istruzioni.
	- **Linux (tranne Steam Deck)**: Seguite le istruzioni per la vostra distribuzione qua: https://docs.microsoft.com/it-it/dotnet/core/install/linux, in particolare dovrete installare il pacchetto **dotnet-runtime-8.0**.
	- **Steam Deck**: Dovrete installare .NET tramite lo script di Microsoft. Segui la guida [qua](https://learn.microsoft.com/en-us/dotnet/core/install/linux-scripted-manual#scripted-install), e installa la versione 8.0.0. (Speriamo di poter mettere a disposizione un modo più facile presto...).
	- **macOS**: Andate sulla [pagina ufficiale](https://dotnet.microsoft.com/download/dotnet/8.0/runtime) e selezionate *Download x64 (Intel)* o *Download Arm64 (Apple Silicon)* sotto *Run apps*, a seconda se state utilizzando un Mac con processore Intel o con processore Apple Silicon (tipo M1, M2, etc.).

## Installazione
- Scaricate [l'ultima release dell'installer](https://github.com/USPAssets/Installer/releases/latest) (la trovate anche sul [sito](https://undertaleita.net/)).
- Decomprimete il contenuto del file in una cartella
- Eseguite **USPInstaller.bat** su *Windows*, eseguite **USPInstaller.sh** su *macOS* o *Linux*.
- Scegliete il gioco per cui volete installare la patch.
- Cliccate su **Sfoglia** e selezionate, dalla cartella dove avete installato il gioco, il file **DELTARUNE.exe** o **UNDERTALE.exe** (o gli eseguibili di referenza nelle altre piattaforme).
- Cliccate **Avvia installazione!**, e attendete
- Appena sara' tutto concluso, il vostro gioco sara' tradotto! Potete ora avviarlo e giocare

## Rimuovere la patch
Il modo migliore di rimuovere la patch è reinstallare il gioco (i vostri salvataggi rimarranno intatti). Su Steam, potete eseguire i seguenti passaggi:
- Tasto destro sul gioco in *Libreria*.
- Cliccate su *Proprietà*.
- Nella barra a sinistra cliccate su *File installati*.
- Cliccate su *Verifica integrità dei file del gioco*.

A questo punto Steam provvederà a riscaricare il gioco e la patch sarà rimossa! 

## Risoluzione problemi
- Se quando provate ad eseguire l'exe su Windows vi si apre un pop-up di *Windows SmartScreen*, cliccate su *Ulteriori Informazioni*, e poi su *Esegui comunque*. 
- Se su macOS o Linux doveste ricevere un avviso di *accesso negato* quando provate ad eseguire lo script, eseguite i seguenti passaggi:
	- Aprite una finestra di terminale, e navigate tramite `cd` nella directory in cui avete decompresso l'installer. Poi eseguite i seguenti comandi:

```
$ chmod a+x USPInstaller.sh
$ ./USPInstaller.sh
```
## Note aggiuntive 
L'installer non potrebbe esistere senza [UndertaleModTool](https://github.com/UnderminersTeam/UndertaleModTool). Un grazie speciale a tutte le persone che hanno contribuito al progetto!

Se doveste avere problemi, [REDACTED]

A presto!

*Renard*