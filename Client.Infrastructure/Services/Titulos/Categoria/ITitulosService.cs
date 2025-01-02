﻿using IGift.Application.CQRS.Titulos.Titulos.Conectado.Query;
using IGift.Application.CQRS.Titulos.Titulos.Desconectado.Query;
using IGift.Application.Responses.Titulos.Categoria;
using IGift.Application.Responses.Titulos.Conectado;
using IGift.Application.Responses.Titulos.Desconectado;
using IGift.Shared.Wrapper;

namespace IGift.Client.Infrastructure.Services.Titulos.Categoria
{
    public interface ITitulosService
    {
        Task<IResult<IEnumerable<CategoriaResponse>>> GetAllCategorias();
        public Task<IResult<IEnumerable<TitulosConectadoResponse>>> GetAllTitulosConectado();
        public Task<IResult<IEnumerable<TitulosDesconectadoResponse>>> GetAllTitulosDesconectado();
    }
}
