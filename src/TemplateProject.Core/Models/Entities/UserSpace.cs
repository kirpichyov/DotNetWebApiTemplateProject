using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace TemplateProject.Core.Models.Entities;

public sealed class UserSpace
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; init; }
    
    public string Name { get; init; }
    
    public DateTime CreatedAtUtc { get; init; }
    
    public Guid UserId { get; init; }
}