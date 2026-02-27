using Konscious.Security.Cryptography;
using System.Text;

namespace CryptoPuzzles.Server.Helpers
{
    public static class Argon2PasswordVerifier
    {
        public static bool VerifyPassword(string password, string storedHash)
        {
            byte[] hashBytes = Convert.FromBase64String(storedHash);

            byte[] salt = new byte[16];
            Array.Copy(hashBytes, 0, salt, 0, 16);

            byte[] hash = new byte[32];
            Array.Copy(hashBytes, 16, hash, 0, 32);

            using var argon2 = new Argon2id(Encoding.UTF8.GetBytes(password));
            argon2.Salt = salt;
            argon2.DegreeOfParallelism = 1;
            argon2.MemorySize = 65536;
            argon2.Iterations = 3;

            byte[] newHash = argon2.GetBytes(32);

            for (int i = 0; i < 32; i++)
            {
                if (hash[i] != newHash[i])
                    return false;
            }
            return true;
        }
    }
}
