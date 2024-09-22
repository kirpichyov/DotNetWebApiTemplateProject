using System;
using System.Threading.Tasks;
using FakeItEasy;
using TemplateProject.Core.Contracts.Repositories;

namespace TemplateProject.UnitTests;

public sealed class UnitOfWorkFakeWrapper
{
    public Fake<IUnitOfWork> Fake { get; }
    public IUnitOfWork FakedObject { get; }

    public Fake<IUserRepository> Users { get; }
    public Fake<IRefreshTokenRepository> RefreshTokens { get; }

    public UnitOfWorkFakeWrapper()
    {
        Fake = new Fake<IUnitOfWork>();
        Users = new Fake<IUserRepository>();
        RefreshTokens = new Fake<IRefreshTokenRepository>();

        Fake.CallsTo(unitOfWork => unitOfWork.Users)
            .Returns(Users.FakedObject);

        Fake.CallsTo(unitOfWork => unitOfWork.RefreshTokens)
            .Returns(RefreshTokens.FakedObject);

        FakedObject = Fake.FakedObject;
    }

    public void SetupCommitTransactionAsyncInvocation<TResult>()
    {
        Fake.CallsTo(unitOfWork => unitOfWork.CommitTransactionAsync(A<Func<TResult>>._))
            .ReturnsLazily(fakeObjectCall => Task.FromResult(fakeObjectCall.GetArgument<Func<TResult>>(0)!.Invoke()));
    }
}