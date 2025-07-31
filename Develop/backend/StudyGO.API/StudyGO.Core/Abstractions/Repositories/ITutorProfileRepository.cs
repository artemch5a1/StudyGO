using StudyGO.Core.Abstractions.Base.DataCrud;
using StudyGO.Core.Models;

namespace StudyGO.Core.Abstractions.Repositories
{
    public interface ITutorProfileRepository : IReadable<TutorProfile>
    {
        bool Create(TutorProfile model);
        bool Update(TutorProfile model);
    }
}
