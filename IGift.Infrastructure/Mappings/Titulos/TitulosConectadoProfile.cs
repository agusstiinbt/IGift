﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using IGift.Application.Models.Titulos;
using IGift.Application.Responses.Titulos.Conectado;

namespace IGift.Infrastructure.Mappings.Titulos
{
    public class TitulosConectadoProfile : Profile
    {
        public TitulosConectadoProfile()
        {
            CreateMap<TitulosConectado, TitulosConectadoResponse>();
        }
    }
}
