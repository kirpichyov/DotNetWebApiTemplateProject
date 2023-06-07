using System.Collections.Generic;
using System.Linq;

namespace TemplateProject.Application.Common.Models;

public class ItemsContainerModel<TItem>
{
    public ItemsContainerModel(long itemsTotalCount, IEnumerable<TItem> items)
    {
        ItemsTotalCount = itemsTotalCount;
        Items = items.ToArray();
    }
    
    public ItemsContainerModel(IEnumerable<TItem> items)
    {
        Items = items.ToArray();
        ItemsTotalCount = Items.Count;
    }

    public ItemsContainerModel()
    {
    }
    
    public long ItemsTotalCount { get; init; }
    public IReadOnlyCollection<TItem> Items { get; init; }
}