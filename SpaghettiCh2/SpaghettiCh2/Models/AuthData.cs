using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace USPInstaller.Models
{
#if QA
    public class AuthData
    {
        private readonly string keyPath;
        private string appId;
        private string installationId;

        public bool IsInitialised { get; private set; }

        private AuthData(string appId, string installationId, string keyPath)
        {
            this.appId = appId ?? string.Empty;
            this.installationId = installationId ?? string.Empty;
            this.keyPath = keyPath ?? string.Empty;
        }

        private AuthData()
        {
            // Error auth data when Init fails
            keyPath = string.Empty;
            appId = string.Empty;
            installationId = string.Empty;
        }

        public static AuthData InitAuthData(string? keyPath, string? detailsFilePath)
        {
            if (!File.Exists(keyPath) || !File.Exists(detailsFilePath))
            {
                Console.WriteLine("Key file or auth file don't exist");
                return new AuthData();
            }

            var deets = File.ReadAllLines(detailsFilePath);
            if (deets.Length < 2)
            {
                Console.WriteLine("Auth file badly formatted");
                return new AuthData();
            }

            AuthData data = new(deets[0], deets[1], keyPath);
            data.IsInitialised = true;

            return data;
        }

        public async Task<string?> GetInstallationToken()
        {
            var jwt = GetJWT();

            using var client = new HttpClient();
            client.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("USPInstaller", "2.0"));
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
            var privateKeyPem = File.ReadAllText(keyPath);
            using var rsa = RSA.Create();
            rsa.ImportFromPem(privateKeyPem.ToCharArray());

            var key = new RsaSecurityKey(rsa);
            var creds = new SigningCredentials(key, SecurityAlgorithms.RsaSha256);
            var now = DateTimeOffset.UtcNow;

            var cl = new Claim[]
            {
                new("iat", now.ToUnixTimeSeconds().ToString())
            };

            var token = new JwtSecurityToken(
                issuer: appId, // iss
                expires: now.UtcDateTime.AddMinutes(9), // exp
                signingCredentials: creds,
                claims: cl
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
#endif
}
