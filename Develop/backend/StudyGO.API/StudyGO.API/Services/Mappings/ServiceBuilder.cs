using StudyGO.Application.Mappings;
using StudyGO.infrastructure.Mappings;

namespace StudyGO.API.Services;

public partial class ServiceBuilder
{
    private void ConfigureAutoMapper()
    {
        _services.AddAutoMapper(cfg =>
        {
            cfg.AddProfile<EntityProfile>();
            cfg.AddProfile<DtosProfile>();
        });
    }
}
