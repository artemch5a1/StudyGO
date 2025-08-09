using AutoMapper;
using StudyGO.Contracts.Dtos.Formats;
using StudyGO.Contracts.Dtos.Subjects;
using StudyGO.Contracts.Dtos.TutorProfiles;
using StudyGO.Contracts.Dtos.UserProfiles;
using StudyGO.Contracts.Dtos.Users;
using StudyGO.Core.Models;

namespace StudyGO.Application.Mappings
{
    public class DtosProfile : Profile
    {
        public DtosProfile()
        {
            ConfigureUsersDto();
            ConfigureSubjectsDto();
            ConfigureFormatDto();
            ConfigureUserProfilesDto();
            ConfigureTutorProfilesDto();
        }

        public void ConfigureUsersDto()
        {
            CreateMap<User, UserDto>();

            CreateMap<UserCreateDto, User>()
                .ForMember(dest => dest.PasswordHash, opt => opt.MapFrom(src => src.Password));

            CreateMap<UserUpdateDto, User>();

            CreateMap<UserUpdateСredentialsDto, User>()
                .ForMember(dest => dest.PasswordHash, opt => opt.MapFrom(src => src.Password));
        }

        public void ConfigureSubjectsDto()
        {
            CreateMap<Subject, SubjectDto>();
            CreateMap<SubjectCreateDto, Subject>();
        }

        public void ConfigureUserProfilesDto()
        {
            CreateMap<UserProfile, UserProfileDto>();

            CreateMap<UserProfileRegistrDto, UserProfile>();

            CreateMap<UserProfileUpdateDto, UserProfile>();
        }

        public void ConfigureFormatDto()
        {
            CreateMap<Format, FormatDto>();
            CreateMap<CreateFormatDto, Format>();
        }

        public void ConfigureTutorProfilesDto()
        {
            CreateMap<TutorProfile, TutorProfileDto>().ForMember(dest => dest.Subjects, 
                opt => 
                opt.MapFrom(src => src.TutorSubjects.Select(x => x.Subject)));
            
            CreateMap<TutorProfileRegistrDto, TutorProfile>().ForMember(dest => dest.TutorSubjects,
                opt => opt
                    .MapFrom(src => src.SubjectsId.Select(x => new TutorSubjects()
                    {
                        SubjectId = x,
                    })));
            CreateMap<TutorProfileUpdateDto, TutorProfile>();
        }
    }
}
