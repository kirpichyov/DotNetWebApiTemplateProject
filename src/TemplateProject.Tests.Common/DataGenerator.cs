using Bogus;
using TemplateProject.Application.Models.Auth;
using TemplateProject.Core.Models.Entities;

namespace TemplateProject.Tests.Common;

public sealed class DataGenerator
{
    private readonly Faker _faker = new();

    public UserRegisterRequest GivenUserRegisterRequest()
    {
        var person = new Person();

        return new UserRegisterRequest()
        {
            Email = person.Email,
            Firstname = person.FirstName,
            Lastname = person.LastName,
            Password = GivenPassword(),
        };
    }

    public User GivenUser()
    {
        var person = new Person();
        return new User(person.FirstName, person.LastName, person.Email, GivenPassword());
    }

    private string GivenPassword()
    {
        return _faker.Random.String2(length: 3).ToUpperInvariant() + _faker.Random.Replace("###????@");
    }
}