using System;
using System.IO;
using System.IO.Compression;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace USPInstaller.Models
{
    public class AssetFolder
    {
        public enum GameType
        {
            Undertale,
            Deltarune
        }


        public static async Task<string> DownloadLatestAsync(string owner, string repo, string? targetPath)
        {
            using HttpClient httpClient = new()
            {
                Timeout = TimeSpan.FromSeconds(10)
            };
            httpClient.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("USPInstaller", "2.0"));

            var assetsPath = Path.Combine(targetPath ?? ".", "Assets");
            Directory.CreateDirectory(assetsPath);

            // Get latest commit info to check version
            var apiUrl = $"https://api.github.com/repos/{owner}/{repo}/commits/main";
            var request = new HttpRequestMessage(HttpMethod.Get, apiUrl);
            var response = await httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var commitInfo = JsonNode.Parse(await response.Content.ReadAsStringAsync());
            var latestSha = commitInfo["sha"].GetValue<string>();

            // Check if we already have this version
            var versionFile = Path.Combine(assetsPath, ".version");
            if (File.Exists(versionFile))
            {
                var existingVersion = await File.ReadAllTextAsync(versionFile);
                if (existingVersion == latestSha)
                {
                    // Check if the folder exists and has content
                    if (Directory.Exists(assetsPath) && Directory.GetFiles(assetsPath).Length > 1)
                    {
                        return assetsPath;
                    }
                }
            }

            // Download the repository
            var zipUrl = $"https://api.github.com/repos/{owner}/{repo}/zipball/main";
            Stream zipStream = await httpClient.GetStreamAsync(zipUrl);
            using (ZipArchive archive = new ZipArchive(zipStream))
            {
                foreach (ZipArchiveEntry entry in archive.Entries)
                {
                    if (entry.FullName.EndsWith("/")) continue; // Skip directories

                    string entryName = entry.FullName;
                    // Trim first directory name (something like Assets-fe00dpb)
                    var baseIndex = entryName.IndexOf('/');
                    if (baseIndex != -1)
                    {
                        entryName = entryName.Substring(baseIndex + 1);
                    }

                    entryName = entryName.Replace('/', Path.DirectorySeparatorChar); // Normalize path separators
                    string destPath = Path.Combine(assetsPath, entryName);
                    Directory.CreateDirectory(Path.GetDirectoryName(destPath));
                    entry.ExtractToFile(destPath);
                    await Task.Yield(); // Yield to avoid blocking the thread
                }
            }

            // Save the version info
            await File.WriteAllTextAsync(versionFile, latestSha);

            return assetsPath;
        }
    }
}
