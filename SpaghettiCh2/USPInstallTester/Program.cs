
using System.ComponentModel;
using USPInstaller.Models;
using USPInstaller.ViewModels;

class TestInstallViewModel : InstallationViewModel
{
    public TestInstallViewModel() : base()
    {
        PropertyChanged += OnPropertyChanged;
    }

    private void OnPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == "OverallProgressMessage")
        {
            Console.WriteLine(OverallProgressMessage);
        }
    }

    public override bool AskUserQuestion(string message)
    {
        // Always assume user selected "yes" to questions
        return true;
    }

    public override void LogError(string s)
    {
        // Treat any error log as a hard error
        throw new Exception("Installation script logged an error: rs" + s);
    }
}

class Program
{
    static string? FindApplicationWithName(string directory, string name)
    {
        string attempt = Path.Join(directory, name + ".exe");
        if (File.Exists(attempt))
        {
            return attempt;
        }
        attempt = Path.Join(directory, name + ".app");
        if (Directory.Exists(attempt))
        {
            return attempt;
        }
        attempt = Path.Join(directory, "run.sh");
        if (File.Exists(attempt))
        {
            return attempt;
        }
        return null;
    }

    static async Task<(string, string)> GetConfig()
    {
        string configPath = Path.Join(AppContext.BaseDirectory, "config.txt");
        if (File.Exists(configPath))
        {
            // Read two lines from configPath
            string[] lines = await File.ReadAllLinesAsync(configPath);
            if (lines.Length >= 2)
            {
                Console.WriteLine("Read configuration from: " + configPath);
                Console.WriteLine("Assets path: " + lines[0]);
                Console.WriteLine("Games root path: " + lines[1]);
                return (lines[0], lines[1]);
            }
        }
        string? assetPath = null;
        while (assetPath == null)
        {
            Console.Write("Asset path: ");
            assetPath = Console.ReadLine();
        }

        string? rootDir = null;
        while (rootDir == null)
        {
            Console.Write("Games root directory: ");
            rootDir = Console.ReadLine();
        }
        await File.WriteAllLinesAsync(configPath, [assetPath, rootDir]);
        Console.WriteLine("Wrote configuration to: " + configPath);
        return (assetPath, rootDir);
    }

    // See https://learn.microsoft.com/en-us/dotnet/standard/io/how-to-copy-directories
    static async Task CopyDirectory(string sourceDir, string destinationDir, bool recursive)
    {
        // Get information about the source directory
        var dir = new DirectoryInfo(sourceDir);

        // Check if the source directory exists
        if (!dir.Exists)
            throw new DirectoryNotFoundException($"Source directory not found: {dir.FullName}");

        // Cache directories before we start copying
        DirectoryInfo[] dirs = dir.GetDirectories();

        // Create the destination directory
        Directory.CreateDirectory(destinationDir);

        // Get the files in the source directory and copy to the destination directory
        foreach (FileInfo file in dir.GetFiles())
        {
            string targetFilePath = Path.Combine(destinationDir, file.Name);
            file.CopyTo(targetFilePath);
        }

        // If recursive and copying subdirectories, recursively call this method
        if (recursive)
        {
            foreach (DirectoryInfo subDir in dirs)
            {
                string newDestinationDir = Path.Combine(destinationDir, subDir.Name);
                await CopyDirectory(subDir.FullName, newDestinationDir, true);
            }
        }
    }

    static async Task CopyToFolder(string directory, string tempPath)
    {
        if (Directory.Exists(tempPath))
            Directory.Delete(tempPath, true);
        await CopyDirectory(directory, tempPath, true);
    }

    static async Task<int> Main(string[] args)
    {
        (string assetPath, string rootDir) = await GetConfig();

        var patchedRootDir = Path.Combine(rootDir, "Patched");
        Directory.CreateDirectory(patchedRootDir);

        InstallationViewModel installer = new TestInstallViewModel();

        bool failed = false;
        foreach (string subDir in Directory.GetDirectories(rootDir))
        {
            try
            {
                string fileName = Path.GetFileName(subDir);
                string patchedCopyDir = Path.Combine(patchedRootDir, fileName);

                string? undertaleExe = FindApplicationWithName(subDir, "UNDERTALE");
                if (undertaleExe != null)
                {
                    await CopyToFolder(subDir, patchedCopyDir);
                    Console.WriteLine($"\nInstalling Undertale patch on {fileName}...");
                    await installer.InstallUndertale(assetPath, FindApplicationWithName(patchedCopyDir, "UNDERTALE")!);
                    continue;
                }
                string? deltaruneExe = FindApplicationWithName(subDir, "DELTARUNE");
                if (deltaruneExe != null)
                {
                    await CopyToFolder(subDir, patchedCopyDir);
                    Console.WriteLine($"\nInstalling Deltarune patch on {fileName}...");
                    await installer.InstallDeltarune(assetPath, FindApplicationWithName(patchedCopyDir, "DELTARUNE")!);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Installation to {Path.GetFileName(subDir)} failed:");
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);
                failed = true;
            }
        }

        if (!failed)
        {
            if (!Console.IsOutputRedirected)
            {
                Console.WriteLine("\nInstallation completed succesfully :D\nPress any key to exit...");
                Console.ReadKey();
            }
            return 0;
        }
        if (!Console.IsOutputRedirected)
        {
            Console.WriteLine("\nThere were some errors during installation :(\nPress any key to exit");
            Console.ReadKey();
        }
        return 10;
    }
}