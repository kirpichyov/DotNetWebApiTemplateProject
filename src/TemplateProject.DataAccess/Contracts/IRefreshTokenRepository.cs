﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TemplateProject.Core.Models.Entities;

namespace TemplateProject.DataAccess.Contracts;

public interface IRefreshTokenRepository : IRepositoryBase<RefreshToken>
{
    Task<RefreshToken> FindById(Guid id, bool useTracking);
    Task<IReadOnlyCollection<RefreshToken>> FindAllByUserId(Guid userId, bool useTracking);
}
