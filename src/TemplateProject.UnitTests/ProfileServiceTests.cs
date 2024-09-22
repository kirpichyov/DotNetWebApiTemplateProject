using System;
using System.Threading.Tasks;
using Bogus;
using FakeItEasy;
using FluentAssertions;
using FluentAssertions.Execution;
using Kirpichyov.FriendlyJwt.Contracts;
using NUnit.Framework;
using TemplateProject.Application.Mapping;
using TemplateProject.Application.Security;
using TemplateProject.Application.Services;
using TemplateProject.Tests.Common;

namespace TemplateProject.UnitTests;

[TestFixture]
public sealed class ProfileServiceTests
{
    private readonly Faker _faker = new();
    private readonly DataGenerator _dataGenerator = new();

    private UnitOfWorkFakeWrapper _unitOfWorkFakeWrapper;
    private Fake<ISecurityContext> _securityContextFake;

    [SetUp]
    public void Setup()
    {
        _unitOfWorkFakeWrapper = new UnitOfWorkFakeWrapper();
        _securityContextFake = new Fake<ISecurityContext>();
    }

    [Test]
    public async Task GetCurrentProfile_UserIsAuthorized_ShouldReturnExpected()
    {
        // Arrange
        var user = _dataGenerator.GivenUser();

        _securityContextFake
            .CallsTo(reader => reader.UserId)
            .Returns(user.Id);
        
        _securityContextFake
            .CallsTo(reader => reader.IsAuthenticated)
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
        _securityContextFake
            .CallsTo(reader => reader.IsAuthenticated)
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
        _securityContextFake.FakedObject);
}