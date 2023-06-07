using System;
using System.Collections.Generic;
using System.Linq;

namespace TemplateProject.Core.Models.Dtos.Common;

public sealed class Page<TItem>
{
    public Page(IEnumerable<TItem> items, int itemsCount, int pageSize, int pageNumber)
    {
        Items = items.ToArray();

        if (itemsCount < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(itemsCount));
        }

        if (pageSize < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(pageSize));
        }

        var fullPages = itemsCount / pageSize;
        var hasUncompletedLastPage = itemsCount % pageSize > 0;
        var lastPage = hasUncompletedLastPage ? 1 : 0;
        var pageCount = fullPages + lastPage;

        ItemsCount = itemsCount;
        PageSize = pageSize;
        PageCount = pageCount;
        PageNumber = pageNumber;
    }
    
    public IReadOnlyCollection<TItem> Items { get; }
    public int ItemsCount { get; }
    public int PageSize { get; }
    public int PageCount { get; }
    public int PageNumber { get; }
}