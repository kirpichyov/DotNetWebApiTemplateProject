using System;
using System.Threading.Tasks;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Quartz;
using TemplateProject.Core.Models.Messaging;
using TemplateProject.DataAccess.Connection;

namespace TemplateProject.Application.Jobs;

public sealed class SampleJob : IJob
{
    private readonly DatabaseContext _databaseContext;
    private readonly ILogger<SampleJob> _logger;
    private readonly IPublishEndpoint  _publishEndpoint;

    public SampleJob(
        DatabaseContext databaseContext,
        ILogger<SampleJob> logger,
        IPublishEndpoint publishEndpoint)
    {
        _databaseContext = databaseContext;
        _logger = logger;
        _publishEndpoint = publishEndpoint;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        _logger.LogInformation("Sample job fired!");

        var usersCount = await _databaseContext.Users.CountAsync();

        await _publishEndpoint.Publish(new SampleCommand
        {
            Id = Guid.NewGuid(),
            UsersCount = usersCount,
        });
    }
}