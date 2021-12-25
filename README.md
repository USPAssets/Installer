
# Utilizzare l'installer universale

Ciao! 
Finalmente abbiamo un installer universale! Con questo potrete patchare *UNDERTALE* e *DELTARUNE* (Capitolo 1 e 2), sia su **Windows**, **macOS** e **Linux**! L'installer vi permettera' anche di aggiornare la patch nel caso rilasceremo aggiornamenti. Insomma, conveniente!

## Compatibilita'

L'installer e' compatibile con le seguenti versioni dei giochi:
-  **DELTARUNE (Chaper 1 & 2 DEMO)**, versione **1.07**
-  **UNDERTALE**, versione **1.08**
E con qualsiasi versione di Windows, macOS o distribuzione Linux in grado di far girare **.NET Runtime 6.0** (piu' info nei prerequisit).

## Prerequisiti

- Come prima cosa, trovate la cartella dove avete installato *UNDERTALE* o *DELTARUNE*; generalmente, se avete installato il gioco tramite Steam, questa sarà:
```
C:\Programmi(x86)\Steam\steamapps\common\Undertale 
// oppure
C:\Programmi(x86)\Steam\steamapps\common\DELTARUNEdemo
```
- Verificate se il vostro sistema è a 32Bit o 64Bit, potete farlo andando su Informazioni sul Sistema dalla barra di ricerca di Windows in basso a sinistra.
- **Solo per Windows e Linux**, nstallate il runtime di .NET 5.0 da qua: https://dotnet.microsoft.com/download/dotnet/5.0/runtime, 
	- **Windows**: Selezionate *Download x64* o *Download x86* sotto *Run desktop apps*, a seconda se avete un sistema a 64 o 32 bit rispettivamente, e installate il runtime seguendo le istruzioni,
	- **Linux**: Seguite le istruzioni per la vostra distribuzione qua: https://docs.microsoft.com/it-it/dotnet/core/install/linux, in particolare dovrete installare il pacchetto **dotnet-runtime-5.0**
	- 
## Installazione su Windows
- Scaricate il file .zip con l'installer dal sito https://undertaleita.net/
- Decomprimete il contenuto del file in una cartella
- Aprite *Italian Patcher by USP*
- Aspettate che l'installer scarichi gli aggiornamenti, poi cliccate su **Installa**.
- Cliccate su **Sfoglia** e selezionate, dalla cartella dove avete installato il gioco, il file **data.win**, o **DELTARUNE.exe** o **UNDERTALE.exe**.
- Cliccate **Applica la patch**, e attendete
- Appena sara' tutto concluso, il vostro gioco sara' tradotto! Potete ora avviarlo e giocare

**NOTA**: Se quando provate ad avviare l'exe Windows vi blocca con un popup di Windows SmartScreen, cliccate su *Ulteriori Informazioni*, e poi su *Esegui comunque*.

## Installazione su macOS
- Scaricate il file .tar con l'installer dal sito https://undertaleita.net/
- Decomprimete il contenuto del file
- Spostate il file *Italian Patch by USP* nella cartella *Applicazioni* (**IMPORTANTE**)
- Aprite il *Finder*, navigate nella cartella *Applicazioni*, fate tasto destro su *Spaghetti Installer* e cliccate su **Apri**.
- Accettate qualsiasi avviso apparira' su schermo.
- Aspettate che l'installer scarichi gli aggiornamenti, poi cliccate su **Installa**.
- Cliccate su **Sfoglia** e selezionate, dalla cartella dove avete installato il gioco, il file **game.ios**, o **DELTARUNE.app** o **UNDERTALE.app**.
- Cliccate **Applica la patch**, e attendete
- Appena sara' tutto concluso, il vostro gioco sara' tradotto! Potete ora avviarlo e giocare

## Installazione su Linux
**NOTA**: Le istruzioni per Linux verranno fornite per essere eseguite tramite terminale.
- Scaricate il file .tar con l'installer dal sito https://undertaleita.net/
- Decomprimete il contenuto del file in una cartella
- **ASSICURATEVI DI AVER INSTALLATO IL RUNTIME DI .NET**, leggi i prerequisiti in caso.
- Aprite una finestra di terminale, e navigate tramite `cd` nella directory in cui avete decompresso, per esempio se ho decompresso il gioco in `~/SpaghettiInstaller` faro':
```
$ cd ~/SpaghettiInstaller
```
- Ora eseguite i seguenti comandi:
```
$ chmod a+x ItalianPatcherLinux.sh
$ ./ItalianPatcherLinux.sh
```
- Aspettate che l'installer scarichi gli aggiornamenti, poi cliccate su **Installa**.
- Cliccate su **Sfoglia** e selezionate, dalla cartella dove avete installato il gioco, il file **game.unx**
- Cliccate **Applica la patch**, e attendete
- Appena sara' tutto concluso, il vostro gioco sara' tradotto! Potete ora avviarlo e giocare

## Note aggiuntive 
L'installer e' stato possibile grazie al nostro [@Nik](https://github.com/nkrapivin). In più, un grazie speciale a [@krzys_h](https://github.com/krzys-h) per aver sviluppato *UndertaleModTool*, che è stato essenziale per lo sviluppo.

Se avete problemi, scriveteci su Facebook o mandateci un'email a *undertalespaghettiproject@gmail.com*. Grazie del supporto e della pazienza! Speriamo possiate divertirvi con DELTARUNE e UNDERTALE in italiano!

A presto!

*Renard*