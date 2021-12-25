/// <summary>
/// Contains all models of this Avalonia application.
/// </summary>
namespace SpaghettiCh2.Models
{
    /// <summary>
    /// Translate these strings please:
    /// (I was too lazy to move them into a file)
    /// </summary>
    public static class StringsModel
    {
        public static string StatusAllGood => "Tutto in ordine!";
        public static string StatusFileDoesNotExist => "Il File non esiste.";
        public static string StatusRunningScript => "Installando la traduzione...";
        public static string StatusLoading => "Caricando file di gioco...";
        public static string StatusSaving => "Salvando, NON chiudere la finestra";
        public static string StatusDone => "Completato. Puoi chiudermi e avviare il gioco!";
        public static string AssetsDoNotExist => "Il percorso degli asset per la traduzione non esiste.\nPer favore,\n ri-scarica l'installer ->";
        public static string AssetsVersionError => "Errore nella lettura della versione degli Asset.\nPer favore,\n ri-scarica l'installer ->";
        public static string ERROR => "ERRORE: {0}";
        public static string StatusNoGame => "Nessun file di gioco valido selezionato, premi Sfoglia";
        public static string DialogFilterName => "File di Gioco (*.exe, *.app, *.win, *.ios, *.unx)";
        public static string DialogTitle => "Seleziona DELTARUNE.exe, .app o data.win";
        public static string WebsiteLink => "https://undertaleita.net/deltarune.html";
        public static string StatusFormat => "Status: {0}";
        public static string UIVersionStringFormat => "Installer Versione: {0}\nAssets Versione: {1}\nDebug Info: {2}";
        public static string UIGreeting => "Undertale/Deltarune Patch Italiana\ndell'Undertale Spaghetti Project";
        public static string UIBelowGreeting => "Scegli cosa fare:";
        public static string UIButtonInstall => "Installa";
        public static string UIButtonWebsite => "Visita il nostro sito";
        public static string UIAboveTextBox => "Percorso del gioco:";
        public static string UIButtonBrowse => "Sfoglia...";
        public static string UIButtonApply => "Applica la patch!";
        // ... or "your game path here"
        public static string UIBoxWatermark => "Premi 'Sfoglia' per scegliere il gioco...";
        public static string UIWindowTitle => "Undertale/Deltarune Patcher dell'USP";
        public static string UIAssetDirRO => "Usando dir. temp. per gli asset";
        public static string UICheckingForUpdates => "Cercando aggiornamenti...";
        public static string UIDownloadingUpdate => "Aggiornamento trovato, in download...";
    }
}