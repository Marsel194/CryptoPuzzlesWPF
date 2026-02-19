using Konscious.Security.Cryptography;
using System.Security.Cryptography;
using System.Text;

internal class Program
{
    private static string HashPassword(string password)
    {
        byte[] salt = new byte[16];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(salt);
        }

        using var argon2 = new Argon2id(Encoding.UTF8.GetBytes(password));
        argon2.Salt = salt;
        argon2.DegreeOfParallelism = 1;
        argon2.MemorySize = 65536;
        argon2.Iterations = 3;

        byte[] hash = argon2.GetBytes(32);

        byte[] hashBytes = new byte[16 + 32];
        Array.Copy(salt, 0, hashBytes, 0, 16);
        Array.Copy(hash, 0, hashBytes, 16, 32);

        return Convert.ToBase64String(hashBytes);
    }

    private static void Main(string[] args)
    {
        Console.Write("Введи пароль для админа: ");
        string password = Console.ReadLine();

        string hash = HashPassword(password);

        Console.WriteLine("\nХэш для базы данных:");
        Console.WriteLine(hash);
    }
}