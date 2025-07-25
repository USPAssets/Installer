using ICSharpCode.SharpZipLib.Zip;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Underanalyzer.Decompiler;
using UndertaleModLib;
using UndertaleModLib.Decompiler;
using UndertaleModLib.Models;
using UndertaleModLib.Scripting;
using USPInstaller.ViewModels;

namespace USPInstaller.Models
{
    class ScriptContext(UndertaleData data, string dataPath, string scriptPath, InstallationViewModel model) : IScriptInterface
    {
        public UndertaleData Data => data;

        public string FilePath => dataPath;

        public string ScriptPath => scriptPath;

        public void ScriptMessage(string message)
        {
            model.Log(message);
        }

        public void ScriptError(string error, string title = "Error", bool SetConsoleText = true)
        {
            model.Log(title + ": " + error);
        }

        public void UpdateProgressBar(string message, string status, double progressValue, double maxValue)
        {
            model.UpdateProgress(message, status, progressValue, maxValue);
        }

        public void UpdateProgressStatus(string status)
        {
            throw new NotImplementedException();
        }

        public void HideProgressBar()
        {
        }

        public bool ScriptQuestion(string message)
        {
            return MessageBoxViewModel.Show(message, "Domanda", true).Result;
        }

        public void UpdateProgressValue(double progressValue)
        {
            throw new NotImplementedException();
        }


        public object Highlighted => throw new NotImplementedException();

        public object Selected => throw new NotImplementedException();

        public bool CanSave => throw new NotImplementedException();

        public bool ScriptExecutionSuccess => throw new NotImplementedException();

        public string ScriptErrorMessage => throw new NotImplementedException();

        public string ExePath => throw new NotImplementedException();

        public string ScriptErrorType => throw new NotImplementedException();

        public bool IsAppClosed => throw new NotImplementedException();

        public void AddProgress(int amount)
        {
            throw new NotImplementedException();
        }

        public void AddProgressParallel(int amount)
        {
            throw new NotImplementedException();
        }

        public void ChangeSelection(object newSelection, bool inNewTab = false)
        {
            throw new NotImplementedException();
        }

        public Task ClickableSearchOutput(string title, string query, int resultsCount, IOrderedEnumerable<KeyValuePair<string, List<(int lineNum, string codeLine)>>> resultsDict, bool showInDecompiledView, IOrderedEnumerable<string> failedList = null)
        {
            throw new NotImplementedException();
        }

        public Task ClickableSearchOutput(string title, string query, int resultsCount, IDictionary<string, List<(int lineNum, string codeLine)>> resultsDict, bool showInDecompiledView, IEnumerable<string> failedList = null)
        {
            throw new NotImplementedException();
        }

        public void DisableAllSyncBindings()
        {
            throw new NotImplementedException();
        }

        public void EnableUI()
        {
            throw new NotImplementedException();
        }

        public string GetDecompiledText(string codeName, GlobalDecompileContext context, IDecompileSettings settings)
        {
            return GetDecompiledText(Data.Code.ByName(codeName), context, settings);
        }
        public string GetDecompiledText(UndertaleCode code, GlobalDecompileContext context, IDecompileSettings settings)
        {
            if (code.ParentEntry is not null)
                return $"// This code entry is a reference to an anonymous function within \"{code.ParentEntry.Name.Content}\", decompile that instead.";

            GlobalDecompileContext globalDecompileContext = context is null ? new(Data) : context;
            try
            {
                return code != null
                    ? new DecompileContext(globalDecompileContext, code, settings ?? Data.ToolInfo.DecompilerSettings).DecompileToString()
                    : "";
            }
            catch (Exception e)
            {
                return "/*\nDECOMPILER FAILED!\n\n" + e.ToString() + "\n*/";
            }
        }

        public string GetDisassemblyText(string codeName)
        {
            throw new NotImplementedException();
        }

        public string GetDisassemblyText(UndertaleCode code)
        {
            throw new NotImplementedException();
        }

        public int GetProgress()
        {
            throw new NotImplementedException();
        }

        public void IncrementProgress()
        {
            throw new NotImplementedException();
        }

        public void IncrementProgressParallel()
        {
            throw new NotImplementedException();
        }

        public void InitializeScriptDialog()
        {
            throw new NotImplementedException();
        }

        public bool LintUMTScript(string path)
        {
            throw new NotImplementedException();
        }

        public bool MakeNewDataFile()
        {
            throw new NotImplementedException();
        }

        public string PromptChooseDirectory()
        {
            throw new NotImplementedException();
        }

        public string PromptLoadFile(string defaultExt, string filter)
        {
            throw new NotImplementedException();
        }

        public string PromptSaveFile(string defaultExt, string filter)
        {
            throw new NotImplementedException();
        }

        public bool RunUMTScript(string path)
        {
            throw new NotImplementedException();
        }

        public string ScriptInputDialog(string title, string label, string defaultInput, string cancelText, string submitText, bool isMultiline, bool preventClose)
        {
            throw new NotImplementedException();
        }

        public void ScriptOpenURL(string url)
        {
            throw new NotImplementedException();
        }

        public void ScriptWarning(string message)
        {
            throw new NotImplementedException();
        }

        public void SetFinishedMessage(bool isFinishedMessageEnabled)
        {
            throw new NotImplementedException();
        }

        public void SetProgress(int value)
        {
            throw new NotImplementedException();
        }

        public void SetProgressBar(string message, string status, double progressValue, double maxValue)
        {
            throw new NotImplementedException();
        }

        public void SetProgressBar()
        {
            throw new NotImplementedException();
        }

        public void SetUMTConsoleText(string message)
        {
            throw new NotImplementedException();
        }

        public string SimpleTextInput(string title, string label, string defaultValue, bool allowMultiline, bool showDialog = true)
        {
            throw new NotImplementedException();
        }

        public void SimpleTextOutput(string title, string label, string message, bool allowMultiline)
        {
            throw new NotImplementedException();
        }

        public void StartProgressBarUpdater()
        {
            throw new NotImplementedException();
        }

        public Task StopProgressBarUpdater()
        {
            throw new NotImplementedException();
        }

        public void SyncBinding(string resourceType, bool enable)
        {
            throw new NotImplementedException();
        }
    }
}
