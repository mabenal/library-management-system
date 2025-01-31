using Microsoft.Extensions.Configuration;

namespace lms.Abstractions
{
    public class ConfigurationManager
    {
        private static readonly Lazy<ConfigurationManager> _instance = new Lazy<ConfigurationManager>(() => new ConfigurationManager());

        public IConfiguration? Configuration { get; private set; }

        private ConfigurationManager() { }

        public static ConfigurationManager GetInstance(IConfiguration configuration)
        {
            _instance.Value.Configuration = configuration;
            return _instance.Value;
        }

        public string GetJwtKey() => Configuration?["Jwt:Key"] ?? throw new InvalidOperationException("Configuration is not set.");
        public string GetJwtIssuer() => Configuration?["Jwt:Issuer"] ?? throw new InvalidOperationException("Configuration is not set.");
        public string GetJwtAudience() => Configuration?["Jwt:Audience"] ?? throw new InvalidOperationException("Configuration is not set.");
    }
}