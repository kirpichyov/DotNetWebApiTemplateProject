namespace TemplateProject.Core.Exceptions;

public sealed class ResourceNotFoundException : CoreException
{
    public ResourceNotFoundException(string itemName)
        : base(new[] { new PropertyErrors(null, $"{itemName} is not found.") }, 
            ExceptionsInfo.Identifiers.ResourceNotFound)
    {
        ItemName = itemName;
    }
    
    public string ItemName { get; }
}