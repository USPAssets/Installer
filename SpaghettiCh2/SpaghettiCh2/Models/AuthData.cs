#if QA

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
    public class AuthData(string appId, string installationId, string keyPath)
    {

        public static AuthData Init(string keyPath, string detailsFilePath)
        {
            var deets = File.ReadAllLines(detailsFilePath);
            if (deets.Length < 2)
            {
                throw new InvalidOperationException("Auth file badly formatted");
            }
            return new AuthData(deets[0], deets[1], keyPath);
        }

        public async Task<string> GetInstallationToken()
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

            if (string.IsNullOrEmpty(token))
            {
                throw new InvalidOperationException("Failed to retrieve installation token");
            }

            return token;
        }

        private string GetJWT()
        {
            var privateKeyPem = File.ReadAllText(keyPath);
            using var rsa = RSA.Create();
            rsa.ImportFromPem(privateKeyPem.ToCharArray());

            var key = new RsaSecurityKey(rsa);
            var creds = new SigningCredentials(key, SecurityAlgorithms.RsaSha256)
            {
                CryptoProviderFactory = new CryptoProviderFactory { CacheSignatureProviders = false }
            };
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
}

#endif