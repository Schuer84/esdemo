using Microsoft.Extensions.DependencyInjection;

namespace SqlStreamStore.Demo
{
    public interface IServiceCollectionConfiguration
    {
        void Configure(IServiceCollection serviceCollection);
    }
}