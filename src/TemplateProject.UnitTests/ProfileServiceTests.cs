using System;
using System.Threading.Tasks;
using Bogus;
using FakeItEasy;
using FluentAssertions;
using FluentAssertions.Execution;
using Kirpichyov.FriendlyJwt.Contracts;
using NUnit.Framework;
using TemplateProject.Application.Mapping;
using TemplateProject.Application.Services;
using TemplateProject.Tests.Common;

namespace TemplateProject.UnitTests;

[TestFixture]
public sealed class ProfileServiceTests
{
    private readonly Faker _faker = new();
    private readonly DataGenerator _dataGenerator = new();

    private UnitOfWorkFakeWrapper _unitOfWorkFakeWrapper;
    private Fake<IJwtTokenReader> _jwtTokenReaderFake;

    [SetUp]
    public void Setup()
    {
        _unitOfWorkFakeWrapper = new UnitOfWorkFakeWrapper();
        _jwtTokenReaderFake = new Fake<IJwtTokenReader>();
    }

    [Test]
    public async Task GetCurrentProfile_UserIsAuthorized_ShouldReturnExpected()
    {
        // Arrange
        var user = _dataGenerator.GivenUser();

        _jwtTokenReaderFake
            .CallsTo(reader => reader.UserId)
            .Returns(user.Id.ToString());
        
        _jwtTokenReaderFake
            .CallsTo(reader => reader.IsLoggedIn)
            .Returns(true);

        _unitOfWorkFakeWrapper.Users
            .CallsTo(repository => repository.TryGet(user.Id, false))
            .Returns(user);

        var sut = BuildSut();

        // Act
        var result = await sut.GetCurrentProfile();

        // Assert
        using var scope = new AssertionScope();
        result.Id.Should().Be(user.Id);
        result.Email.Should().Be(user.Email);
        result.Firstname.Should().Be(user.Firstname);
        result.Lastname.Should().Be(user.Lastname);
    }
    
    [Test]
    public async Task GetCurrentProfile_UserIsUnauthorized_ShouldThrowInvalidOperationException()
    {
        // Arrange
        _jwtTokenReaderFake
            .CallsTo(reader => reader.IsLoggedIn)
            .Returns(false);

        var sut = BuildSut();

        // Act
        var func = async () => await sut.GetCurrentProfile();

        // Assert
        using var scope = new AssertionScope();
        await func.Should().ThrowAsync<InvalidOperationException>();
    }

    public ProfileService BuildSut() => new(
        _unitOfWorkFakeWrapper.FakedObject,
        new ObjectsMapper(),
        _jwtTokenReaderFake.FakedObject);
}