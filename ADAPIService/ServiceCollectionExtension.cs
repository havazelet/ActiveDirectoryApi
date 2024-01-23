using ADAPIReposetory.implementions;
using ADAPIReposetory.interfaces;
using ADAPIService.implementations;
using ADAPIService.interfaces;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtension
{
    public static IServiceCollection AddService(this IServiceCollection services)
    {
        services.AddSingleton<IServiceInterface, ADService>();
        return services;
    } 
}
