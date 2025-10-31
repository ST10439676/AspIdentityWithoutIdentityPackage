using System;
using System.Security.Cryptography;
using MvcIdentiyFirstPrinciples.Models;
using MvcIdentiyFirstPrinciples.Services;

namespace MvcIdentiyFirstPrinciples;


public class UserTestFactory(IPasswordHasher passwordHasher) {
    public User CreateUser(User user, string password)
    {
        (string hash, string salt) = passwordHasher.GenerateHash(password);
        user.PasswordHash = hash;
        user.Salt = salt;
        return user;
    }
}