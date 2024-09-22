namespace TemplateProject.Core.Contracts.DataManipulation;

public interface IPageFilter<TItem> : IExpressionFilter<TItem>
{
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
}