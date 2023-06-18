using AutoMapper;
using BusinessLogic.Contracts;
using WebApi.Models;

namespace WebApi.Mapping
{
    /// <summary>
    /// Профиль автомаппера для сущности курса.
    /// </summary>
    public class CourseMappingsProfile : Profile
    {
        public CourseMappingsProfile()
        {
            CreateMap<CourseDto, CourseModel>();
            CreateMap<CourseModel, CourseDto>()
                .ForMember(t=>t.Deleted, r=> r.Ignore());
            CreateMap<AddCourseModel, CourseDto>()
                .ForMember(t=>t.Id, r=> r.Ignore())
                .ForMember(t=>t.Lessons, r=> r.Ignore())
                .ForMember(t=>t.Deleted, r=> r.Ignore());
        }
    }
}
