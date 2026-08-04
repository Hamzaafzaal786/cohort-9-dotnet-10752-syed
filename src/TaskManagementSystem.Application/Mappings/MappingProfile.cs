using AutoMapper;
using TaskManagementSystem.Application.DTOs.Auth;
using TaskManagementSystem.Application.DTOs.Task;
using TaskManagementSystem.Application.DTOs.User;
// Use ALIAS to avoid confusion with System.Threading.Tasks.Task
using TaskEntity = TaskManagementSystem.Domain.Entities.Task;

namespace TaskManagementSystem.Application.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // User mappings
            CreateMap<Domain.Entities.User, UserDto>();

            // Task mappings - use TaskEntity alias
            CreateMap<TaskEntity, TaskDto>()
                .ForMember(dest => dest.UserName,
                    opt => opt.MapFrom(src => src.User != null ? src.User.FullName : null));

            CreateMap<CreateTaskDto, TaskEntity>()
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow));

            CreateMap<UpdateTaskDto, TaskEntity>()
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow));

            // Auth mappings
            CreateMap<RegisterDto, Domain.Entities.User>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.Email));
        }
    }
}