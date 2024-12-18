﻿using Microsoft.Extensions.DependencyInjection;

namespace RevitLookup2.Services;

public static class ViewModelServices
{
    public static void RegisterViewModels(this IServiceCollection services)
    {
        services.Scan(selector => selector.FromCallingAssembly()
            .AddClasses(filter => filter.Where(type => type.Name.EndsWith("ViewModel")))
            .AsImplementedInterfaces(type => type.Name.EndsWith("ViewModel"))
            .WithScopedLifetime());
    }
}