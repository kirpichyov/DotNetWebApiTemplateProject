using System.Threading.Tasks;
using NUnit.Framework;
using TemplateProject.IntegrationTests.Testing;

namespace TemplateProject.IntegrationTests;

[SetUpFixture]
public class Setup
{
    [OneTimeSetUp]
    public async Task GlobalSetup()
    {
        await SystemUnderTest.StartAsync();
    }

    [OneTimeTearDown]
    public async Task GlobalTeardown()
    {
        await SystemUnderTest.ShutdownAsync();
    }
}