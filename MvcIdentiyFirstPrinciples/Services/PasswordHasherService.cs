using System;
using Microsoft.Extensions.Options;
using System.Security.Cryptography;
using System.ComponentModel.DataAnnotations;

namespace MvcIdentiyFirstPrinciples.Services;



public interface IPasswordHasher
{
    public (string hash, string salt) GenerateHash(string password);
    public bool VerifyPassword(string password, string hash, string salt);
}

public class Pbkdf2PasswordHasherOptions
{
    public HashAlgorithmName HashAlgorithmName { get; set; }
    public int Iterations { get; set; }
    public int OutputLength { get; set; }
}

/**
<summary>
Password hashing implementation using <see cref="Rfc2898DeriveBytes.Pbkdf2"/>
</summary>
*/
public class Pbkdf2PasswordHasherService : IPasswordHasher
{

    private Random _random = new();

    private Pbkdf2PasswordHasherOptions _options;

    public Pbkdf2PasswordHasherService(IOptions<Pbkdf2PasswordHasherOptions> options)
    {
        _options = options.Value;
    }

    private byte[] Hash(string password, byte[] salt)
    {
        return Rfc2898DeriveBytes.Pbkdf2(password, salt, _options.Iterations, _options.HashAlgorithmName, _options.OutputLength);
    }

    public (string hash, string salt) GenerateHash(string password)
    {
        byte[] saltBytes = new byte[16];
        _random.NextBytes(saltBytes);

        byte[] hash = Hash(password, saltBytes);

        return (Convert.ToBase64String(hash), Convert.ToBase64String(saltBytes));
    }

    public bool VerifyPassword(string password, string hash, string salt)
    {
        byte[] saltBytes = Convert.FromBase64String(salt);

        var testHash = Rfc2898DeriveBytes.Pbkdf2(password, saltBytes, 100_000, HashAlgorithmName.SHA256, 32);

        return Convert.ToBase64String(testHash) == hash;
    }

}
