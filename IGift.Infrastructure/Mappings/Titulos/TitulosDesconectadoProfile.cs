﻿using AutoMapper;
using IGift.Application.Models.Titulos;
using IGift.Application.Responses.Titulos.Desconectado;

namespace IGift.Infrastructure.Mappings.Titulos
{
    public class TitulosDesconectadoProfile : Profile
    {
        public TitulosDesconectadoProfile()
        {
            CreateMap<TitulosDesconectado, TitulosDesconectadoResponse>();
        }
    }
}
