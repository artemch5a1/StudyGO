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

        private void ConfigureUsersDto()
        {
            CreateMap<User, UserDto>();

            CreateMap<UserUpdateDto, User>().ConvertUsing( x => User.UpdateUser(
                x.UserId,
                x.Surname,
                x.Name,
                x.Patronymic,
                x.Number
                ));
        }

        private void ConfigureSubjectsDto()
        {
            CreateMap<Subject, SubjectDto>();
            CreateMap<SubjectCreateDto, Subject>();
        }

        private void ConfigureUserProfilesDto()
        {
            CreateMap<UserProfile, UserProfileDto>();

            CreateMap<UserProfileRegistrDto, UserProfile>().ConvertUsing(x => 
                UserProfile.CreateUser(
                    x.User.Email,
                    x.User.Password,
                    x.User.Surname,
                    x.User.Name,
                    x.User.Patronymic,
                    x.User.Number,
                    x.DateBirth,
                    x.SubjectId,
                    x.Description
                    )
                );

            CreateMap<UserProfileUpdateDto, UserProfile>().ConvertUsing(x => 
                UserProfile.UpdateUser(
                    x.UserId,
                    x.DateBirth,
                    x.SubjectId,
                    x.Description
                    )
                );
        }

        private void ConfigureFormatDto()
        {
            CreateMap<Format, FormatDto>();
            CreateMap<CreateFormatDto, Format>();
        }

        private void ConfigureTutorProfilesDto()
        {
            CreateMap<TutorProfile, TutorProfileDto>().ForMember(dest => dest.Subjects, 
                opt => 
                opt.MapFrom(src => src.TutorSubjects.Select(x => x.Subject)));
            
            CreateMap<TutorProfileRegistrDto, TutorProfile>().ConvertUsing(x => 
                TutorProfile.CreateTutor
                    (
                        x.User.Email,
                        x.User.Password,
                        x.User.Surname,
                        x.User.Name,
                        x.User.Patronymic,
                        x.User.Number,
                        x.Bio,
                        x.PricePerHour,
                        x.City,
                        x.FormatId,
                        x.SubjectsId
                        )
                );
            
            CreateMap<TutorProfileUpdateDto, TutorProfile>().ConvertUsing(x => 
                TutorProfile.UpdateTutor
                (
                    x.UserId,
                    x.Bio,
                    x.PricePerHour,
                    x.City,
                    x.FormatId,
                    x.SubjectsId
                    )
                );
        }
    }
}
