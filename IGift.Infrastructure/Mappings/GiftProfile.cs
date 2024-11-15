﻿using AutoMapper;
using IGift.Application.Requests.Peticiones.Command;
using IGift.Domain.Entities;

namespace IGift.Infrastructure.Mappings
{
    public class GiftProfile : Profile
    {

        public GiftProfile()
        {
            CreateMap<Peticiones, AddEditPeticionesCommand>();
        }
    }
}
