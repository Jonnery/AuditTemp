using Audit.Realization.Configure;
using Audit.Realization.Extensions;
using Microsoft.Extensions.Options;

namespace Audit.Realization.Options;

public interface IOptionsDbConfiguration
{
    void ConfigureDbContext();
}

public class OptionsConfiguration : IOptionsDbConfiguration
{
    private readonly DatabaseOption _databaseOptions;
    private readonly IServiceProvider _serviceProvider;

    public OptionsConfiguration(IOptions<DatabaseOption> databaseOptions, IServiceProvider serviceProvider)
    {
        _databaseOptions = databaseOptions?.Value;
        _serviceProvider = serviceProvider;
    }

    public void ConfigureDbContext()
    {
        // 获取数据库选项
        
        DatabaseConfig.InitConfiguration(_databaseOptions);
    }
}
