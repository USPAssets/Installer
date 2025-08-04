
# USPInstaller - Patcher universale per DELTARUNE e UNDERTALE in italiano by USP

Ciao! 
Finalmente abbiamo un installer universale! Con questo potrete patchare *UNDERTALE* e *DELTARUNE* (Capitolo 1 e 2), sia su **Windows**, **macOS** e **Linux**! L'installer vi permettera' anche di aggiornare la patch nel caso rilasceremo aggiornamenti. Insomma, conveniente!

## Compatibilita'

L'installer e' compatibile con le seguenti versioni dei giochi:
- **DELTARUNE (Chaper 1 & 2 DEMO)**, verione Itch o Steam.
- **DELTARUNE**, testato con:
	- Capitolo 1 v1.38
	- Capitolo 2 v1.44
- **UNDERTALE**, v1.08

L'installer è compatibile con Windows, macOS e Linux (Steam Deck incluso).

## Prerequisiti

- Se avete installato la patch in precedenza, reinstallate il gioco seguendo le istruzioni nella [sezione qui sotto](#rimuovere-la-patch).
- Trovate dove avete installato *UNDERTALE* o *DELTARUNE*; se avete il gioco su Steam potete andare in *Libreria*, cliccare col tasto destro sul gioco e selezionare *Sfoglia file locali*.
- *(Solo per Windows e macOS)* - Installate il **runtime di .NET 8.0**:
	- **Windows**: Andate sulla [pagina ufficiale](https://dotnet.microsoft.com/download/dotnet/8.0/runtime) e selezionate *Download x64* o *Download x86* sotto *Run desktop apps*, a seconda se avete un sistema a 64 o 32 bit rispettivamente, e installate il runtime seguendo le istruzioni.
	- **macOS**: Andate sulla [pagina ufficiale](https://dotnet.microsoft.com/download/dotnet/8.0/runtime) e selezionate *Download x64 (Intel)* o *Download Arm64 (Apple Silicon)* sotto *Run apps*, a seconda se state utilizzando un Mac con processore Intel o con processore Apple Silicon (tipo M1, M2, etc.).

## Installazione
- Scaricate [l'ultima release dell'installer](https://github.com/USPAssets/Installer/releases/latest) (la trovate anche sul [sito](https://undertaleita.net/)).
- Decomprimete il contenuto del file in una cartella
- Eseguite **USPInstaller.exe** su *Windows*, eseguite **USPInstaller** su *Linux* e **USPInstaller.command** su *macOS*.
- Scegliete il gioco per cui volete installare la patch.
- Inserite il percorso all'eseguibile del gioco:
	- **(Windows/Linux)**: Cliccate su **Sfoglia** e selezionate, dalla cartella dove avete installato il gioco, il file **DELTARUNE.exe** o **UNDERTALE.exe**.
		- **NOTA PER UNDERTALE E LINUX**: UNDERTALE ha una versione nativa su Linux. Se la state utilizzando, potete selezionare in quel caso *runner* o *run.sh* nella directory del gioco. 
	- **(macOS)**: Copiate e incollate il percorso di **UNDERTALE.app** o **DELTARUNE.app** nel box. Potete cliccare col tasto destro sull'app, tenere premuto il pusalnte *Opzione* sulla tastiera e cliccare su *Copia nome_del_file come percorso*.
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
- Se ricevete un avviso che vi dice che il file va prima estratto, seguite la guida apposita [qui](https://github.com/USPAssets/Installer/blob/main/GUIDA_ESTRAZIONE_UT.md).
- Se su macOS ricevete un avviso che non vi permette di aprire l'installer, seguite le [istruzioni di Apple](https://support.apple.com/it-it/102445) per aprire l'app, con particolare attenzione alla sezione *Se vuoi aprire un'app non autenticata o proveniente da uno sviluppatore non identificato*.
- Se quando provate ad eseguire l'exe su Windows vi si apre un pop-up di *Windows SmartScreen*, cliccate su *Ulteriori Informazioni*, e poi su *Esegui comunque*.

## Note aggiuntive 
L'installer non potrebbe esistere senza [UndertaleModTool](https://github.com/UnderminersTeam/UndertaleModTool). Un grazie speciale a tutte le persone che hanno contribuito al progetto!

Se doveste avere problemi, [HYPERLINK_BLOCKED]

A presto!

*Renard*
