using System.Security.Cryptography;
using System.Text;

namespace AuthorizationAPI.Services.StaticHelpers
{
    internal static class Hacher
    {
        public static string StringToHach(string message)
        {
            using (var hashAlg = MD5.Create())
            {
                byte[] hash = hashAlg.ComputeHash(Encoding.UTF8.GetBytes(message));
                return Convert.ToBase64String(hash);
            }
        }
    }
}
