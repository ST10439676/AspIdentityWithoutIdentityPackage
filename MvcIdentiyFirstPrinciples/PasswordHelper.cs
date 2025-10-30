using System;
using System.Security.Cryptography;
using MvcIdentiyFirstPrinciples.Models;

namespace MvcIdentiyFirstPrinciples;

public static class PasswordHelper
{
    private static Random _random = new();
    public static (string hash, string salt) GenerateHash(string password)
    {
        byte[] saltBytes = new byte[16];
        _random.NextBytes(saltBytes);
        Console.WriteLine(Convert.ToBase64String(saltBytes));

        byte[] hash = Rfc2898DeriveBytes.Pbkdf2(password, saltBytes, 100_000, HashAlgorithmName.SHA256, 32);

        return (Convert.ToBase64String(hash), Convert.ToBase64String(saltBytes));
    }
    
    public static bool VerifyPassword(string password, string hash, string salt)
    {
        byte[] saltBytes = Convert.FromBase64String(salt);

        var testHash = Rfc2898DeriveBytes.Pbkdf2(password, saltBytes, 100_000, HashAlgorithmName.SHA256, 32);

        return Convert.ToBase64String(testHash) == hash;
    }
}

public static class UserTestFactory {
    public static User CreateUser(User user, string password)
    {
        (string hash, string salt) = PasswordHelper.GenerateHash(password);
        user.PasswordHash = hash;
        user.Salt = salt;
        return user;
    }
}