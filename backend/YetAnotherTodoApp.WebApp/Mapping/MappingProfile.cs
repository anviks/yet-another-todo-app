using AutoMapper;
using WebApp.Dtos;
using YetAnotherTodoApp.Core.Entities;

namespace WebApp.Mapping;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<TodoTaskDto, TodoTask>();
    }
}