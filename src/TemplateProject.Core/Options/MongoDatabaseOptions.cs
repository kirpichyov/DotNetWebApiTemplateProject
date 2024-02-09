namespace TemplateProject.Core.Options;

public class MongoDatabaseOptions
{
    public string ConnectionString { get; set; }
    public string DatabaseName { get; set; }
    public string UserNotesCollectionName { get; set; }
}