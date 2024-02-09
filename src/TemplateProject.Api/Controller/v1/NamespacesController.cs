using System;
using System.Linq;
using System.Threading.Tasks;
using Kirpichyov.FriendlyJwt.Contracts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TemplateProject.Application.Extensions;
using TemplateProject.Application.Models.UserNamespaces;
using TemplateProject.Core.Models.Entities;
using TemplateProject.DataAccess.Connection;

namespace TemplateProject.Api.Controller.v1;

public sealed class NamespacesController : ApiControllerBase
{
    private readonly MongoDbContext _mongoDbContext;
    private readonly IJwtTokenReader _jwtTokenReader;

    public NamespacesController(
        MongoDbContext mongoDbContext,
        IJwtTokenReader jwtTokenReader)
    {
        _mongoDbContext = mongoDbContext;
        _jwtTokenReader = jwtTokenReader;
    }

    [HttpPost]
    public async Task<IActionResult> CreateNamespace([FromBody] CreateNamespaceRequest request)
    {
        var userSpace = new UserSpace()
        {
            Name = request.Name,
            UserId = _jwtTokenReader.GetUserId(),
            CreatedAtUtc = DateTime.UtcNow,
        };

        await _mongoDbContext.UserSpaces.AddAsync(userSpace);

        return StatusCode(StatusCodes.Status201Created, userSpace);
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var userSpaces = await _mongoDbContext.UserSpaces
            .Where(us => us.UserId == _jwtTokenReader.GetUserId())
            .ToListAsync();

        return Ok(new { Data = userSpaces });
    }
}