using AutoMapper;
using StudyGO.Core.Enums;
using StudyGO.Core.Extensions;
using StudyGO.Core.Models;
using StudyGO.infrastructure.Entities;

namespace StudyGO.infrastructure.Mappings
{
    public class EntityProfile : Profile
    {
        public EntityProfile()
        {
            ConfigureUsersMappings();
            ConfigureSubjectMappings();
            ConfigureUserProfileMappings();
            ConfigureFormatMappings();
            ConfigureTutorProfileMappings();
            ConfigureTutorSubjectsMappings();
        }

        private void ConfigureUsersMappings()
        {
            CreateMap<User, UserEntity>();

            CreateMap<UserEntity, User>()
                .ConvertUsing(x => User.MapToUserFromEntity(
                    x.UserId,
                    x.Email,
                    x.PasswordHash,
                    x.Role.GetRolesEnum() ?? RolesEnum.User,
                    x.Surname,
                    x.Name,
                    x.Patronymic,
                    x.Number,
                    x.DateRegistry,
                    x.Verified,
                    x.VerifiedDate
                ));
        }

        private void ConfigureSubjectMappings()
        {
            CreateMap<Subject, SubjectEntity>();
            CreateMap<SubjectEntity, Subject>();
        }

        private void ConfigureUserProfileMappings()
        {
            CreateMap<UserProfile, UserProfileEntity>();

            CreateMap<UserProfileEntity, UserProfile>();
        }

        private void ConfigureFormatMappings()
        {
            CreateMap<Format, FormatEntity>();
            CreateMap<FormatEntity, Format>();
        }

        private void ConfigureTutorProfileMappings()
        {
            CreateMap<TutorProfile, TutorProfileEntity>();
            CreateMap<TutorProfileEntity, TutorProfile>();
        }

        private void ConfigureTutorSubjectsMappings()
        {
            CreateMap<TutorSubjects, TutorSubjectsEntity>();
            CreateMap<TutorSubjectsEntity, TutorSubjects>();
        }
    }
}
