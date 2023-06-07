using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TemplateProject.Core.Models.Dtos.Common;

namespace TemplateProject.DataAccess.DataManipulation;

public class PageFilter<TItem> : IPageFilter<TItem>
{
    public Expression<Func<TItem, bool>> FilteringExpression { get; set; }
    public Expression<Func<TItem, object>> OrderingExpression { get; set; }
    public OrderingDirection OrderingDirection { get; set; }
    
    public int PageNumber { get; set; }
    public int PageSize { get; set; }

    public async Task<Page<TItem>> ApplyToQueryable(IQueryable<TItem> source)
    {
        if (FilteringExpression is not null)
        {
            source = source.Where(FilteringExpression);
        }

        if (OrderingExpression is not null)
        {
            source = OrderingDirection is OrderingDirection.Ascending
                ? source.OrderBy(OrderingExpression)
                : source.OrderByDescending(OrderingExpression);
        }

        var skip = PageNumber < 1
            ? 0
            : (PageNumber - 1) * PageSize;

        var itemsCount = await source.CountAsync();
        var pagedItems = await source
            .Skip(skip)
            .Take(PageSize)
            .ToArrayAsync();

        return new Page<TItem>(pagedItems, itemsCount, PageSize, PageNumber);
    }
}