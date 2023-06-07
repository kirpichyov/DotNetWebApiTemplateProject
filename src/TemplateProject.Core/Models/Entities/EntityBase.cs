namespace TemplateProject.Core.Models.Entities;

public class EntityBase<TId>
{
	protected EntityBase(TId id)
	{
		Id = id;
	}

	protected EntityBase()
	{
	}

	public TId Id { get; }
}
