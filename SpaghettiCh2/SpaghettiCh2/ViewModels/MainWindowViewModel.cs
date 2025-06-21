#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
#pragma warning disable CS0649 // Null thing.
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Threading;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using ReactiveUI;
using SpaghettiCh2.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using UndertaleModLib;
using UndertaleModLib.Scripting;
using Underanalyzer.Decompiler;
using UndertaleModLib.Decompiler;
using UndertaleModLib.Models;


namespace SpaghettiCh2.ViewModels
{
    public class MainWindowViewModel : ViewModelBase, IScriptInterface
    {

        private ScriptOptions CliScriptOptions { get; }
        public bool GMLCacheEnabled { get; set; }
        public bool IsAppClosed { get; set; }
        public UndertaleData Data { get; set; }
        public object Selected { get; set; }
        private int progressValue;

        public bool MakeNewDataFile()
        {
            Data = UndertaleData.CreateNew();
            Console.WriteLine("New file created.");
            return true;
        }

        public void ScriptWarning(string message)
        {
            Console.WriteLine($"WARNING: {message}");
        }

        public void SetProgressBar()
        {
            //no progress bar that can be setup to show
        }
        public void IncrementProgress()
        {
            progressValue++;
        }
        public void AddProgressParallel(int amount) //P - Parallel (multi-threaded)
        {
            Interlocked.Add(ref progressValue, amount); //thread-safe add operation (not the same as "lock ()")
        }

        public void IncrementProgressParallel()
        {
            Interlocked.Increment(ref progressValue); //thread-safe increment
        }

        /// <inheritdoc/>
        public int GetProgress()
        {
            return progressValue;
        }

        /// <inheritdoc/>
        public void SetProgress(int value)
        {
            progressValue = value;
        }


        #region Empty Inherited Methods

        /// <inheritdoc/>
        public void InitializeScriptDialog()
        {
            // CLI has no dialogs to initialize
        }

        /// <inheritdoc/>
        public void HideProgressBar()
        {
            // nothing to hide..
        }

        /// <inheritdoc/>
        public void EnableUI()
        {
            // nothing to enable...
        }

        /// <inheritdoc/>
        public void SyncBinding(string resourceType, bool enable)
        {
            //there is no UI with any data binding
        }

        /// <inheritdoc/>
        public void DisableAllSyncBindings()
        {
            //there is no UI with any data binding
        }

        public void StartProgressBarUpdater()
        {

        }

        public async Task StopProgressBarUpdater() //"async" because "Wait()" blocks UI thread
        {
        }

        public void ChangeSelection(object newSelection, bool inNewTab = false)
        {
            //this does *not* make sense, as CLI does not have any selections
            //however, since Selection is a public object, it could potentially be used by scripts
            Selected = newSelection;
        }

        public string PromptChooseDirectory()
        {
            string path;
            DirectoryInfo directoryInfo;
            do
            {
                Console.WriteLine("Please enter a path (or drag and drop) to a valid directory:");
                Console.Write("Path: ");
                path = RemoveQuotes(Console.ReadLine());
                if (string.IsNullOrEmpty(path))
                {
                    return null;
                }
                directoryInfo = new DirectoryInfo(path);
            }
            while (!directoryInfo.Exists);

            return path;
        }

        private static string RemoveQuotes(string s)

        {
            return s.Trim('"', '\'');
        }

        public string PromptSaveFile(string defaultExt, string filter)
        {
            string path;
            do
            {
                Console.WriteLine("Please enter a path (or drag and drop) to save the file:");
                Console.Write("Path: ");
                path = RemoveQuotes(Console.ReadLine());

                if (Directory.Exists(path))
                {
                    Console.WriteLine("Error: Directory exists at that path.");
                    path = null; // Ensuring that the loop will work correctly
                    continue;
                }
            }
            while (string.IsNullOrWhiteSpace(path));

            return path;
        }



        public string GetDecompiledText(string codeName, GlobalDecompileContext context = null, IDecompileSettings settings = null)
        {
            return GetDecompiledText(Data.Code.ByName(codeName), context, settings);
        }
        public string GetDecompiledText(UndertaleCode code, GlobalDecompileContext context = null, IDecompileSettings settings = null)
        {
            if (code.ParentEntry is not null)
                return $"// This code entry is a reference to an anonymous function within \"{code.ParentEntry.Name.Content}\", decompile that instead.";

            GlobalDecompileContext decompileContext = context is null ? new(Data) : context;
            try
            {
                return code != null
                    ? new DecompileContext(decompileContext, code, settings ?? Data.ToolInfo.DecompilerSettings).DecompileToString()
                    : "";
            }
            catch (Exception e)
            {
                return "/*\nDECOMPILER FAILED!\n\n" + e + "\n*/";
            }
        }

        public async Task ClickableSearchOutput(string title, string query, int resultsCount, IOrderedEnumerable<KeyValuePair<string, List<(int lineNum, string codeLine)>>> resultsDict, bool editorDecompile, IOrderedEnumerable<string> failedList = null)
        {
            await ClickableSearchOutput(title, query, resultsCount, resultsDict.ToDictionary(pair => pair.Key, pair => pair.Value), editorDecompile, failedList);
        }

        /// <inheritdoc/>
        public async Task ClickableSearchOutput(string title, string query, int resultsCount, IDictionary<string, List<(int lineNum, string codeLine)>> resultsDict, bool editorDecompile, IEnumerable<string> failedList = null)
        {
            await Task.Delay(1); //dummy await

            // If we have failed entries...
            if (failedList is not null)
            {
                // Convert list to array first
                string[] failedArray = failedList.ToArray();

                // ...Print them all out
                Console.ForegroundColor = ConsoleColor.Red;
                if (failedArray.Length == 1)
                    Console.Error.WriteLine("There is 1 code entry that encountered an error while searching:");
                else
                    Console.Error.WriteLine($"There are {failedArray.Length} code entries that encountered an error while searching");

                foreach (string failedEntry in failedArray)
                    Console.Error.WriteLine(failedEntry);

                Console.ResetColor();
                Console.WriteLine();
            }

            Console.WriteLine($"{resultsCount} results in {resultsDict.Count} code entries for \"{query}\".");
            Console.WriteLine();

            // Print in a pattern of:
            // Results in code_file
            // Line 3: line of code
            // Line 6: line of code
            //
            // Results in code_file_1
            // etc.
            foreach (var dictEntry in resultsDict)
            {
                Console.WriteLine($"Results in {dictEntry.Key}:");
                foreach (var resultEntry in dictEntry.Value)
                    Console.WriteLine($"Line {resultEntry.lineNum}: {resultEntry.codeLine}");

                Console.WriteLine();
            }
        }

        private static async Task<bool> DirectoryCheck(string dirr)
        {
            try
            {
                var path = Path.Combine(dirr, "test.txt");
                await File.WriteAllTextAsync(path, string.Empty);
                File.Delete(path);
                return true;
            }
            catch
            {
                return false;
            }
        }

        private static void DirectoryCopy(string sourceDirName, string destDirName, bool copySubDirs)
        {
            // Get the subdirectories for the specified directory.
            DirectoryInfo dir = new DirectoryInfo(sourceDirName);

            if (!dir.Exists)
            {
                throw new DirectoryNotFoundException(
                    "Source directory does not exist or could not be found: "
                    + sourceDirName);
            }

            DirectoryInfo[] dirs = dir.GetDirectories();

            // If the destination directory doesn't exist, create it.       
            Directory.CreateDirectory(destDirName);

            // Get the files in the directory and copy them to the new location.
            FileInfo[] files = dir.GetFiles();
            foreach (FileInfo file in files)
            {
                string tempPath = Path.Combine(destDirName, file.Name);
                file.CopyTo(tempPath, false);
            }

            // If copying subdirectories, copy them and their contents to new location.
            if (copySubDirs)
            {
                foreach (DirectoryInfo subdir in dirs)
                {
                    string tempPath = Path.Combine(destDirName, subdir.Name);
                    DirectoryCopy(subdir.FullName, tempPath, copySubDirs);
                }
            }
        }

        public string WindowTitle => StringsModel.UIWindowTitle;
        public string WebsiteUrl => StringsModel.WebsiteLink;
        private HttpClient MyClient;

        public static void OpenBrowser(string url)
        {
            try
            {
                Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
            }
            catch
            {
                // hack because of this: https://github.com/dotnet/corefx/issues/10361
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux) || RuntimeInformation.IsOSPlatform(OSPlatform.FreeBSD))
                {
                    Process.Start("xdg-open", url);
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                {
                    Process.Start("open", url);
                }
                else
                {
                    throw;
                }
            }
        }

        public IBrush StatusBrush_;
        public IBrush StatusBrush { get => StatusBrush_; set { this.RaiseAndSetIfChanged(ref StatusBrush_, value); } }

        private bool IsMainVisible_;
        public bool IsMainVisible { get => IsMainVisible_; set { this.RaiseAndSetIfChanged(ref IsMainVisible_, value); } }

        private bool DoItEnabled_;
        public bool DoItEnabled { get => DoItEnabled_; set { this.RaiseAndSetIfChanged(ref DoItEnabled_, value); } }

        private bool BrowseEnabled_;
        public bool BrowseEnabled { get => BrowseEnabled_; set { this.RaiseAndSetIfChanged(ref BrowseEnabled_, value); } }

        private bool TextBoxEnabled_;
        public bool TextBoxEnabled { get => TextBoxEnabled_; set { this.RaiseAndSetIfChanged(ref TextBoxEnabled_, value); } }

        private string StatusText_ = "";
        public string StatusText { get => StatusText_; set { this.RaiseAndSetIfChanged(ref StatusText_, value); } }

        private string TextBoxContent_ = "";
        public string TextBoxContent { get => TextBoxContent_; set { this.RaiseAndSetIfChanged(ref TextBoxContent_, value); } }

        public string SetStatus { get => StatusText; set { StatusText = string.Format(StringsModel.StatusFormat, value); ; } }

        public ICommand InstallCommand { get; }
        public ICommand VisitCommand { get; }
        public ICommand BrowseCommand { get; }
        public ICommand DoItCommand { get; }

        public OpenFileDialog? BrowseGameDialog { get; set; }
        public string BrowseDialogTitle => StringsModel.DialogTitle;

        private bool First_ = false;

        public string GetDataFileName()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                return "game.ios";
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux) || RuntimeInformation.IsOSPlatform(OSPlatform.FreeBSD))
            {
                return "game.unx";
            }
            else
            {
                return "data.win";
            }
        }

        public void OnTextBoxUpdate(string newval)
        {
            // ignore first value (provided by OnNext callback)
            if (!First_)
            {
                First_ = true;
                return;
            }

            //                           specifically allow choosing a directory only on OSX
            //                           since file open dialog treats .app as 'files' even though they're folders...
            // mac moment.
            if (File.Exists(newval) || (Directory.Exists(newval) && RuntimeInformation.IsOSPlatform(OSPlatform.OSX)))
            {
                if (Path.GetExtension(newval) is string extn && Path.GetDirectoryName(newval) is string dirn)
                {
                    if (extn == ".exe" || extn == string.Empty)
                    {
                        // for an .app it returns a prefix.
                        newval = Path.Combine(dirn, GetDataFileName());
                    }
                    else if (extn == ".app")
                    {
                        newval = Path.Combine(newval, "Contents", "Resources", GetDataFileName());
                        // TODO: handle other macOS stuff???
                    }

                    TextBoxContent = newval;
                }

                SetStatus = StringsModel.StatusAllGood;
                DoItEnabled = true;
                StatusBrush = Brushes.White;
            }
            else
            {
                SetStatus = StringsModel.StatusFileDoesNotExist;
                DoItEnabled = false;
                StatusBrush = Brushes.Red;
            }
        }

        private readonly ScriptOptions? scriptOptions_ = null;


        private async Task RunScript(string csxpath)
        {
            ScriptExecutionSuccess_ = true;
            ScriptErrorMessage_ = "";
            ScriptErrorType_ = "";
            ScriptPath_ = csxpath;
            SetStatus = StringsModel.StatusRunningScript;
            StatusBrush = Brushes.White;
            string code = await File.ReadAllTextAsync(csxpath, Encoding.UTF8);
            await CSharpScript.EvaluateAsync(code, scriptOptions_, ((IScriptInterface)this), typeof(IScriptInterface));
        }

        public void HideUI()
        {
            BrowseEnabled = false;
            TextBoxEnabled = false;
            DoItEnabled = false;
        }

        public void RestoreUI()
        {
            BrowseEnabled = true;
            TextBoxEnabled = true;
            DoItEnabled = true;
        }

        public async Task OnPatchStart(string winfile)
        {
            try
            {
                SetStatus = StringsModel.StatusLoading;
                FilePath_ = winfile;

                var fs = new FileStream(FilePath_, FileMode.Open, FileAccess.Read, FileShare.Read);
                Data_ = UndertaleIO.Read(fs, null, null);
                fs.Close();


                string csxpath;
                string assetdir = AssetDir;
                if (Data_.GeneralInfo.FileName.Content.ToLowerInvariant().Contains("deltarune"))
                {
                    csxpath = Path.Combine(assetdir, "Deltarune", "DTScript.csx");
                }
                else
                {
                    csxpath = Path.Combine(assetdir, "Undertale", "UTScript.csx");
                }

                // the fun:
                await RunScript(csxpath);

                if (string.IsNullOrWhiteSpace(ScriptErrorMessage_))
                {
                    SetStatus = StringsModel.StatusSaving;
                    await Task.Run(() =>
                    {
                        using var stream = new FileStream(FilePath_.Replace(".win", "temp.win"), FileMode.Create, FileAccess.Write, FileShare.Read);
                        //Path.GetFileNameWithoutExtension
                        UndertaleIO.Write(stream, Data_,
                            messageHandler: (string msg) => SetStatus = msg);
                    });

                    // ...
                    SetStatus = StringsModel.StatusDone;
                    //RestoreUI();
                }
            }
            catch (Exception exc)
            {
                SetStatus = string.Format(StringsModel.ERROR, exc);
                StatusBrush = Brushes.Red;
                Trace.WriteLine(exc);
                //RestoreUI();
            }
        }

        public Window? MyWindow { get; set; }

        private bool AssetError_;
        public bool AssetError { get => AssetError_; set { this.RaiseAndSetIfChanged(ref AssetError_, value); } }

        private string? DebugCustomMsg_ = null;
        public string? DebugCustomMsg { get => DebugCustomMsg_; set { this.RaiseAndSetIfChanged(ref DebugCustomMsg_, value); } }

        private string AssetDir_ = "";
        public string AssetDir { get => AssetDir_; set { this.RaiseAndSetIfChanged(ref AssetDir_, value); } }

        private string AssetErrorMessage_ = "";
        public string AssetErrorMessage { get => AssetErrorMessage_; set { this.RaiseAndSetIfChanged(ref AssetErrorMessage_, value); } }

        private string AssetVersion_ = "";
        public string AssetVersion { get => AssetVersion_; set { this.RaiseAndSetIfChanged(ref AssetVersion_, value); } }

        public string GetDebugString()
        {
            if (DebugCustomMsg != null)
                return DebugCustomMsg;

            return "-";
        }

        public async Task CheckAssetsUpdate()
        {
            try
            {
                AssetError = true;
                AssetErrorMessage = StringsModel.UICheckingForUpdates;

                MyClient.Timeout = TimeSpan.FromSeconds(4.0);
                // MyClient.DefaultRequestHeaders.Add("Authorization", "token [[REDACTED]]");
                MyClient.DefaultRequestHeaders.CacheControl = new CacheControlHeaderValue() { NoCache = true };
                var baseurl = "https://raw.githubusercontent.com/USPAssets/Online/main/";
                var reply = await MyClient.GetStringAsync(baseurl + "Version.txt");

                if (string.IsNullOrWhiteSpace(reply))
                {
                    throw new Exception("Online version = null...?");
                }

                var myver = new Version(AssetVersion);
                var onlinever = new Version(reply);

                if (onlinever > myver)
                {
                    AssetErrorMessage = StringsModel.UIDownloadingUpdate;
                    // need to autoupdate...
                    var zipreq = await MyClient.GetStreamAsync("https://codeload.github.com/USPAssets/Online/zip/refs/heads/main");
                    if (zipreq is null)
                    {
                        throw new Exception("Online zip error :(");
                    }

                    var tempdir = Path.Combine(Path.GetTempPath(), "_USPUPDATE" + new Random().Next().ToString());
                    Directory.CreateDirectory(tempdir);
                    var zippath = Path.Combine(tempdir, "update.zip");
                    var filespath = Path.Combine(tempdir, "_files");

                    var fs = new FileStream(zippath, FileMode.Create, FileAccess.Write, FileShare.None, 4096, true);
                    await zipreq.CopyToAsync(fs);
                    await zipreq.DisposeAsync();
                    await fs.FlushAsync();
                    await fs.DisposeAsync();

                    ZipFile.ExtractToDirectory(zippath, filespath, Encoding.UTF8, true);
                    File.Delete(zippath);

                    var assetspath = Path.Combine(filespath, "Online-main");

                    // if NOT OSX and Directory is fine
                    if (!RuntimeInformation.IsOSPlatform(OSPlatform.OSX) && await DirectoryCheck(AssetDir))
                    {
                        Directory.Delete(AssetDir, true);
                        Directory.CreateDirectory(AssetDir);
                        DirectoryCopy(assetspath, AssetDir, true);
                        Directory.Delete(tempdir, true);
                    }
                    else
                    {
                        // switch to a temp dir.
                        AssetDir = assetspath;
                        DebugCustomMsg = StringsModel.UIAssetDirRO;
                    }

                    AssetVersion = reply.Trim();
                }

                AssetError = false;
            }
            catch
            {
                // failed to check for online assets, that's fine, just skip.
                AssetError = false;
                // VS stuff...
                if (Debugger.IsAttached) throw;
            }

            VersionString = string.Format(StringsModel.UIVersionStringFormat, InstallerVer, AssetVersion, GetDebugString());
        }

        public string InstallerVer => Assembly.GetExecutingAssembly().GetName().Version?.ToString() ?? "1.0.0.0";

        public bool DetermineAssetsVersion()
        {
            try
            {
                string mydir = AppContext.BaseDirectory;
                AssetDir = Path.Combine(mydir, "USPAssets");
                AssetError = true;

                if (!Directory.Exists(AssetDir))
                {
                    AssetErrorMessage = StringsModel.AssetsDoNotExist;
                }
                else
                {
                    string verinfopath = Path.Combine(AssetDir, "Version.txt");
                    try
                    {
                        try
                        {
                            AssetVersion = File.ReadAllText(verinfopath);
                            if (string.IsNullOrWhiteSpace(AssetVersion))
                            {
                                throw new InvalidOperationException("...");
                            }
                        }
                        catch
                        {
                            // the version will be 0.0 which will force a redownload.
                            AssetVersion = "0.0";
                        }

                        AssetError = false;
                        VersionString = string.Format(StringsModel.UIVersionStringFormat, InstallerVer, AssetVersion, GetDebugString());
                        Dispatcher.UIThread.InvokeAsync(CheckAssetsUpdate, DispatcherPriority.SystemIdle);
                        return true;
                    }
                    catch
                    {
                        AssetErrorMessage = StringsModel.AssetsVersionError;
                    }
                }
            }
            catch (Exception exc)
            {
                AssetError = true;
                AssetErrorMessage = $"{exc.Message}";
            }

            return false;
        }

        public async Task PatchCommandStart()
        {
            HideUI();
            StatusBrush = Brushes.White;
            SetStatus = "Applying...";
            await OnPatchStart(TextBoxContent);
            RestoreUI();
        }

        public MainWindowViewModel()
        {
            MyClient = new HttpClient();
            AssetError = true;
            AssetErrorMessage = "";
            StatusBrush = Brushes.White;
            SetStatus = StringsModel.StatusNoGame;
            IsMainVisible = true;
            BrowseEnabled = true;
            TextBoxEnabled = true;
            DoItEnabled = false;

            BrowseGameDialog = new OpenFileDialog();
            BrowseGameDialog.Title = BrowseDialogTitle;
            BrowseGameDialog.AllowMultiple = false;
            BrowseGameDialog.Filters.Add
            (
                new FileDialogFilter()
                {
                    Name = StringsModel.DialogFilterName,
                    Extensions = new List<string>() { "exe", "app", "win", "ios", "unx" }
                }
            );

            InstallCommand = ReactiveCommand.Create(
                () =>
                {
                    // install is clicked here...
                    IsMainVisible = false;
                }
                );

            VisitCommand = ReactiveCommand.Create(
                () =>
                {
                    // visit is clicked here...
                    OpenBrowser(WebsiteUrl);
                }
                );

            BrowseCommand = ReactiveCommand.CreateFromTask(
                async () =>
                {
                    try
                    {
                        if (MyWindow is null) return;
                        // browse...
                        var lines = await BrowseGameDialog.ShowAsync(MyWindow);
                        // do nothing if did not get the filename...
                        if (lines is null || lines.Length < 1 || lines[0] is null) return;
                        // update if OK.
                        var myline = lines[0];
                        TextBoxContent = myline;
                    }
                    catch
                    {
                        // ignore on any weird browse errors...
                    }
                }
                );

            DoItCommand = ReactiveCommand.Create(
                () =>
                {
                    // apply the patch.
                    Dispatcher.UIThread.InvokeAsync(PatchCommandStart);
                }
                );


            scriptOptions_ = ScriptOptions.Default
                .AddImports("UndertaleModLib", "UndertaleModLib.Models", "UndertaleModLib.Decompiler",
                            "UndertaleModLib.Scripting", "UndertaleModLib.Compiler",
                            "System", "System.IO", "System.Collections.Generic", "System.Linq",
                            "System.Text.RegularExpressions", Assembly.GetExecutingAssembly().GetName().Name)
                .AddReferences(typeof(UndertaleObject).GetTypeInfo().Assembly,
                                Assembly.GetExecutingAssembly(),
                                typeof(System.Text.RegularExpressions.Regex).GetTypeInfo().Assembly
                                )
                .WithEmitDebugInformation(true)
                .WithLanguageVersion(Microsoft.CodeAnalysis.CSharp.LanguageVersion.Latest)
                .WithAllowUnsafe(true); //when script throws an exception, add a exception location (line number)

            this.WhenAnyValue(x => x.TextBoxContent).Subscribe(x => OnTextBoxUpdate(x));

            DetermineAssetsVersion();
        }

        public string Greeting => StringsModel.UIGreeting;

        public string BelowGreeting => StringsModel.UIBelowGreeting;

        public string InstallButtonText => StringsModel.UIButtonInstall;

        public string VisitButtonText => StringsModel.UIButtonWebsite;

        public string AboveTextBox => StringsModel.UIAboveTextBox;

        public string BrowseText => StringsModel.UIButtonBrowse;

        public string ApplyPatch => StringsModel.UIButtonApply;

        /// <summary>
        /// visible if no game path was detected.
        /// </summary>
        public string TextBoxWatermark => StringsModel.UIBoxWatermark;

        private string VersionString_ = "";
        /// <summary>
        /// just this once.
        /// </summary>
        public string VersionString { get => VersionString_; set { this.RaiseAndSetIfChanged(ref VersionString_, value); } }

        UndertaleData Data_;
        UndertaleData IScriptInterface.Data => Data_;

        string FilePath_ = "";
        string IScriptInterface.FilePath => FilePath_;

        public string ScriptPath_ = "";
        string IScriptInterface.ScriptPath => ScriptPath_;

        object Highlighted_;
        object IScriptInterface.Highlighted => Highlighted_;

        object Selected_;
        object IScriptInterface.Selected => Selected_;

        bool CanSave_ = false;
        bool IScriptInterface.CanSave => CanSave_;

        bool ScriptExecutionSuccess_ = false;
        bool IScriptInterface.ScriptExecutionSuccess => ScriptExecutionSuccess_;

        string ScriptErrorMessage_ = "";
        string IScriptInterface.ScriptErrorMessage => ScriptErrorMessage_;

        string ExePath_ = "";
        string IScriptInterface.ExePath => ExePath_;

        string ScriptErrorType_ = "";
        string IScriptInterface.ScriptErrorType => ScriptErrorType_;

        void IScriptInterface.EnsureDataLoaded()
        {
            if (Data_ == null)
                throw new ScriptException("Please load data.win first!");
        }

        void IScriptInterface.ScriptMessage(string message)
        {
            SetStatus = message;
        }

        void IScriptInterface.SetUMTConsoleText(string message)
        {
            Debug.WriteLine(message);
        }

        bool IScriptInterface.ScriptQuestion(string message)
        {
            return true;
        }

        void IScriptInterface.ScriptError(string error, string title, bool SetConsoleText)
        {
            SetStatus = $"{title}: {error}";
            StatusBrush = Brushes.Red;
            ScriptErrorMessage_ = error;
        }

        void IScriptInterface.ScriptOpenURL(string url)
        {
            OpenBrowser(url);
        }

        public bool RunUMTScript(string path)
        {
            
            RunCSharpFile(path);
            return true;
            
        }

        private void RunCSharpFile(string path)
        {
            string lines;
            try
            {
                lines = File.ReadAllText(path, Encoding.UTF8);
            }
            catch (Exception exc)
            {
                // rethrow as otherwise this will get interpreted as success
                Console.Error.WriteLine(exc.Message);
                throw;
            }

            lines = $"#line 1 \"{path}\"\n" + lines;
            ScriptPath_ = path;
            RunCSharpCode(lines, path);
        }

        public string ScriptErrorMessage { get; set; }
        public bool ScriptExecutionSuccess { get; set; }
        public string ScriptPath { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }


        private void RunCSharpCode(string code, string scriptFile = null)
        {

            CSharpScript.EvaluateAsync(code, scriptOptions_, this, typeof(IScriptInterface)).GetAwaiter().GetResult();
    
        }

        bool IScriptInterface.LintUMTScript(string path)
        {
            throw new NotImplementedException();
        }

        void IScriptInterface.InitializeScriptDialog()
        {
            throw new NotImplementedException();
        }

        public string GetDisassemblyText(string codeName)
        {
            return GetDisassemblyText(Data.Code.ByName(codeName));
        }

        /// <inheritdoc/>
        public string GetDisassemblyText(UndertaleCode code)
        {
            if (code.ParentEntry is not null)
                return $"; This code entry is a reference to an anonymous function within \"{code.ParentEntry.Name.Content}\", disassemble that instead.";

            try
            {
                return code != null ? code.Disassemble(Data.Variables, Data.CodeLocals?.For(code), Data.CodeLocals is null) : "";
            }
            catch (Exception e)
            {
                return "/*\nDISASSEMBLY FAILED!\n\n" + e + "\n*/"; // Please don't
            }
        }

        bool IScriptInterface.AreFilesIdentical(string File01, string File02)
        {
            try
            {
                using var fs1 = new FileStream(File01, FileMode.Open, FileAccess.Read, FileShare.Read);
                using var fs2 = new FileStream(File02, FileMode.Open, FileAccess.Read, FileShare.Read);
                if (fs1.Length != fs2.Length) return false;

                while (true)
                {
                    int v1 = fs1.ReadByte(), v2 = fs2.ReadByte();
                    if (v1 != v2) return false;
                    else if (v1 == -1 || v2 == -1) break;
                }

                return true;
            }
            catch
            {
                return false;
            }
        }

        string IScriptInterface.ScriptInputDialog(string titleText, string labelText, string defaultInputBoxText, string cancelButtonText, string submitButtonText, bool isMultiline, bool preventClose)
        {
            throw new NotImplementedException();
        }

        string IScriptInterface.SimpleTextInput(string title, string label, string defaultValue, bool allowMultiline, bool showDialog)
        {
            throw new NotImplementedException();
        }

        void IScriptInterface.SimpleTextOutput(string title, string label, string defaultText, bool allowMultiline)
        {
            throw new NotImplementedException();
        }

        void IScriptInterface.SetFinishedMessage(bool isFinishedMessageEnabled)
        {
            throw new NotImplementedException();
        }

        void IScriptInterface.UpdateProgressBar(string message, string status, double progressValue, double maxValue)
        {
            SetStatus = $"{message}: {status} ({(int)(progressValue / maxValue) * 100}%)";
        }

        void IScriptInterface.SetProgressBar(string message, string status, double progressValue, double maxValue)
        {
            SetStatus = $"{message}: {status} ({(int)(progressValue / maxValue) * 100}%)";
        }

        void IScriptInterface.UpdateProgressValue(double progressValue)
        {
            throw new NotImplementedException();
        }

        void IScriptInterface.UpdateProgressStatus(string status)
        {
            throw new NotImplementedException();
        }

        void IScriptInterface.AddProgress(int amount)
        {
            throw new NotImplementedException();
        }

        int IScriptInterface.GetProgress()
        {
            throw new NotImplementedException();
        }

        void IScriptInterface.SetProgress(int value)
        {
            throw new NotImplementedException();
        }

        void IScriptInterface.HideProgressBar()
        {
            // do nothing...
        }

        void IScriptInterface.EnableUI()
        {
            // ui is always enabled...
        }

        void IScriptInterface.SyncBinding(string resourceType, bool enable)
        {
            throw new NotImplementedException();
        }

        string IScriptInterface.PromptLoadFile(string defaultExt, string filter)
        {
            string path;
            FileInfo fileInfo;
            do
            {
                Console.WriteLine("Please enter a path (or drag and drop) to a valid file:");
                Console.Write("Path: ");
                path = RemoveQuotes(Console.ReadLine());
                if (string.IsNullOrEmpty(path))
                {
                    return null;
                }
                fileInfo = new FileInfo(path);
            }
            while (fileInfo.Exists);

            return path;
        }
    }
    #endregion
}