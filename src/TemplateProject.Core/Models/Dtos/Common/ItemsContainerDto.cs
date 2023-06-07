using System.Collections.Generic;

namespace TemplateProject.Core.Models.Dtos.Common;

public sealed class ItemsContainerDto<TItem>
{
    public long Count { get; init; }
    public IReadOnlyCollection<TItem> Items { get; init; }
}