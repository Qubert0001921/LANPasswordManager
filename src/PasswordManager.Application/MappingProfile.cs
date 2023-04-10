using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using AutoMapper;

using PasswordManager.Application.Dtos;
using PasswordManager.Domain.Entities;

namespace PasswordManager.Application;

public class MappingProfile : Profile
{
    public MappingProfile()
    {         
        CreateMap<PasswordDto, Password>();
        CreateMap<Password, PasswordDto>();
        CreateMap<Role, RoleDto>();
        CreateMap<RoleDto, Role>();
        CreateMap<PasswordGroup, PasswordGroupDto>();
    }
}
