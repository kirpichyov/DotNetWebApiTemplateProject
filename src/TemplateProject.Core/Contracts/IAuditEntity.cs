using System;

namespace TemplateProject.Core.Contracts;

public interface IAuditEntity
{
    public DateTime CreatedAtUtc { get; set; }
    public DateTime? UpdatedAtUtc { get; set; }
    public string CreatedBy { get; set; }
    public string UpdatedBy { get; set; }
}