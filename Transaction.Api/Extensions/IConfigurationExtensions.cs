using Microsoft.Extensions.Configuration;

namespace Transaction.Api.Extensions
{
    public static class IConfigurationExtensions
    {
        public static T BindObject<T>(this IConfiguration configuration, T configObject)
        {
            configuration.GetSection(configObject.GetType().Name).Bind(configObject);
            return configObject;
        }
    }
}
