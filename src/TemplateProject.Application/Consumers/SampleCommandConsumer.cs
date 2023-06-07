using System;
using System.Threading.Tasks;
using MassTransit;
using Microsoft.Extensions.Logging;
using TemplateProject.Core.Models.Messaging;

namespace TemplateProject.Application.Consumers;

public sealed class SampleCommandConsumer : IConsumer<SampleCommand>
{
    private readonly ILogger<SampleCommandConsumer> _logger;

    public SampleCommandConsumer(
        ILogger<SampleCommandConsumer> logger)
    {
        _logger = logger;
    }

    public Task Consume(ConsumeContext<SampleCommand> context)
    {
        ArgumentNullException.ThrowIfNull(context.Message, nameof(context.Message));

        _logger.LogInformation("Command '{Id}' received. Users count: {Count}", context.Message.Id, context.Message.UsersCount);
        return Task.CompletedTask;
    }
}