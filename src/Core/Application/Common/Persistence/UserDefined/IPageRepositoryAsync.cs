﻿using System.Linq.Expressions;

namespace ICISAdminPortal.Application.Common.Persistence.UserDefined;
public interface IPageRepositoryAsync : IRepositoryWithEvents<Page>
{
    Task<bool> IsUniquePageAsync(string name, DefaultIdType moduleIed);

    Task<Page> FindWithIncludesAsync(Expression<Func<Page, bool>> criteria, string[] includes = null);
}
