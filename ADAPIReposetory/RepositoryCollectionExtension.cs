using ADAPIReposetory.implementions;
using ADAPIReposetory.interfaces;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Extensions.DependencyInjection;

public static class RepositoryCollectionExtension
{
    public static IServiceCollection AddRepository(this IServiceCollection services)
    {
        services.AddSingleton<IRepository, Repository>();
        return services;
    }
}
