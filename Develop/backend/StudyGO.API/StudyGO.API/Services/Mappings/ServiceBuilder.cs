using StudyGO.infrastructure.Mappings;

namespace StudyGO.API.Services;

public partial class ServiceBuilder
{
    public static void ConfigureAutoMapper(IServiceCollection services)
    {
        services.AddAutoMapper(cfg =>
        {
            cfg.AddProfile<EntityProfile>();
        });
    }
}
