using System;

namespace TemplateProject.Core.Models.Entities;

public sealed class User : EntityBase<Guid>
{
    public User(string firstname, string lastname, string email, string passwordHash)
        : base(Guid.NewGuid())
    {
        Firstname = firstname;
        Lastname = lastname;
        Email = email;
        PasswordHash = passwordHash;
    }

    private User()
    {
    }

    public string Firstname { get; set; }
    public string Lastname { get; set; }

    public string Fullname => string.IsNullOrEmpty(Lastname)
        ? Firstname
        : $"{Firstname} {Lastname}";
    
    public string Email { get; }
    public string PasswordHash { get; }
}