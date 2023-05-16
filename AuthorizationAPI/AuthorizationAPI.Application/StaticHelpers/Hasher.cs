using System.Security.Cryptography;
using System.Text;

namespace AuthorizationAPI.Application.StaticHelpers
{
    internal static class Hasher
    {
        public static string StringToHash(string message)
        {
            using (var hashAlg = MD5.Create())
            {
                byte[] hash = hashAlg.ComputeHash(Encoding.UTF8.GetBytes(message));
                return Convert.ToBase64String(hash);
            }
        }
    }
}
