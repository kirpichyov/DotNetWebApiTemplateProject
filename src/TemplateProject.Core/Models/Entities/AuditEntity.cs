using System;
using TemplateProject.Core.Contracts;

namespace TemplateProject.Core.Models.Entities;

public abstract class AuditEntity<T> : EntityBase<T>, IAuditEntity
{
    protected AuditEntity(T id)
        : base(id)
    {
    }

    protected AuditEntity()
    {
    }

    public DateTime CreatedAtUtc { get; set; }
    public DateTime? UpdatedAtUtc { get; set; }
    public string CreatedBy { get; set; }
    public string UpdatedBy { get; set; }
}