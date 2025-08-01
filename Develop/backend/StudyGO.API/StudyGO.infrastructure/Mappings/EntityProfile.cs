using AutoMapper;
using StudyGO.Core.Enums;
using StudyGO.Core.Extensions;
using StudyGO.Core.Models;
using StudyGO.infrastructure.Entites;

namespace StudyGO.infrastructure.Mappings
{
    public class EntityProfile : Profile
    {
        public EntityProfile()
        {
            ConfigureUsersMappings();
            ConfigureSubjectMappings();
            ConfigureUserProfileMappings();
        }

        public void ConfigureUsersMappings()
        {
            CreateMap<User, UserEntity>();

            CreateMap<UserEntity, User>()
                .ConstructUsing(x => new User(
                    x.UserID,
                    x.Email,
                    x.PasswordHash,
                    x.Role.GetRolesEnum() ?? RolesEnum.user,
                    x.Surname,
                    x.Name,
                    x.Patronymic,
                    x.Number
                ));
        }

        public void ConfigureSubjectMappings()
        {
            CreateMap<Subject, SubjectEntity>();
            CreateMap<SubjectEntity, Subject>();
        }

        public void ConfigureUserProfileMappings()
        {
            CreateMap<UserProfile, UserProfileEntity>()
                .ForMember(dest => dest.UserID, opt => opt.MapFrom(src => src.User.UserID))
                .ForMember(
                    dest => dest.SubjectID,
                    opt =>
                        opt.MapFrom(src =>
                            src.FavoriteSubject != null ? src.FavoriteSubject.SubjectID : new Guid()
                        )
                );

            CreateMap<UserProfileEntity, UserProfile>();
        }
    }
}
