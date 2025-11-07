using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace USPInstaller.Models
{
    public class AuthData
    {
        private readonly string keyPath;
        private readonly string deetsPath;
        private string appId;
        private string installationId;

        public bool IsInitialised { get; private set; }

        public AuthData(string? keyPath, string? detailsFilePath)
        {
            this.keyPath = keyPath ?? string.Empty;
            this.deetsPath = detailsFilePath ?? string.Empty;
            appId = string.Empty;
            installationId = string.Empty;
        }

        public async Task Init()
        {
            if (IsInitialised)
            {
                return;
            }

            if (!File.Exists(keyPath) || !File.Exists(deetsPath))
            {
                Console.WriteLine("Key file or auth file don't exist");
                return;
            }

            var deets = await File.ReadAllLinesAsync(deetsPath);
            if (deets.Length < 2)
            {
                Console.WriteLine("Auth file badly formatted");
                return;
            }

            appId = deets[0];
            installationId = deets[1];

            IsInitialised = true;
        }

        public async Task<string?> GetInstallationToken()
        {
            var jwt = GetJWT();

            using var client = new HttpClient();
            client.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("Playtest Installer Access", "1.0"));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwt);
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/vnd.github+json"));

            var url = $"https://api.github.com/app/installations/{installationId}/access_tokens";
            var response = await client.PostAsync(url, null);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            var doc = JsonNode.Parse(json);
            var token = doc?["token"]?.GetValue<string>();

            return token;
        }

        private string GetJWT()
        {
            Func<byte[], string> base64UrlEncode = input => Convert.ToBase64String(input).TrimEnd('=').Replace('+', '-').Replace('/', '_');

            var privateKeyPem = File.ReadAllText(keyPath);
            using var rsa = RSA.Create();
            rsa.ImportFromPem(privateKeyPem.ToCharArray());

            var now = DateTimeOffset.UtcNow;
            var header = base64UrlEncode(Encoding.UTF8.GetBytes("{\"alg\":\"RS256\",\"typ\":\"JWT\"}"));
            var payload = base64UrlEncode(Encoding.UTF8.GetBytes($"{{\"iat\":{now.ToUnixTimeSeconds()},\"exp\":{now.AddMinutes(10).ToUnixTimeSeconds()},\"iss\":{appId}}}"));

            var data = $"{header}.{payload}";
            var signature = rsa.SignData(Encoding.UTF8.GetBytes(data), HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
            var signed = $"{data}.{base64UrlEncode(signature)}";

            return signed;
        }
    }
}
