using Isopoh.Cryptography.Argon2;

internal class Program
{
    private static void Main(string[] args)
    {
        Console.Write("Введи пароль для админа: ");
        string password = Console.ReadLine();

        string hash = Argon2.Hash(password);

        Console.WriteLine("\nХэш для базы данных:");
        Console.WriteLine(hash);
    }
}