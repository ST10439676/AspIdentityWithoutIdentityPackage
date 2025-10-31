using System.ComponentModel.DataAnnotations;
using Microsoft.Extensions.Options;
using MvcIdentiyFirstPrinciples.Models;

namespace MvcIdentiyFirstPrinciples.Services;

public class UserDbOptions
{
    public User[] Users { get; set; }
}

public class UserDb
{
    private List<User> _users = new List<User>();

    public UserDb(IOptions<UserDbOptions> options)
    {
        UserDbOptions userDbOptions = options.Value;
        ValidateUsers([.. userDbOptions.Users]);

        _users.AddRange(userDbOptions.Users);
    }

    private void ValidateUser(User user)
    {
        var context = new ValidationContext(user);
        Validator.ValidateObject(user, context, true);
    }
    
    private void ValidateUsers(List<User> users)
    {
        var context = new ValidationContext(users);
        Validator.ValidateObject(users, context, true);     
    }

    public User? GetUserByName(string username)
    {
        return (from user in _users where user.Username == username select user).FirstOrDefault();
    }
    public User? GetUserById(int id)
    {
        return (from user in _users where user.UserId == id select user).FirstOrDefault();
    }

    public void AddUser(User user)
    {
        user.UserId = _users.Count;
        ValidateUser(user);

        _users.Add(user);
    }
}
