﻿using AutoMapper;
using IGift.Application.Interfaces.Files;
using IGift.Application.Interfaces.Repositories;
using IGift.Application.Requests.Files;
using IGift.Domain.Entities;
using IGift.Shared;
using IGift.Shared.Wrapper;
using MediatR;
using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.Net.Http.Headers;
namespace IGift.Application.Features.LocalesAdheridos
{
    public class AddEditLocalAdheridoCommand : IRequest<IResult>
    {
        /// <summary>
        /// Si se envía igual a 0(cero) significa que estamos editando un local
        /// </summary>
        public int Id { get; set; }
        [Required]
        public string Nombre { get; set; } = string.Empty;
        public string CreatedBy { get; set; } = AppConstants.AdminEmail;
        public string Descripcion { get; set; } = string.Empty;
        public string ImageDataURL { get; set; } = string.Empty;
        public UploadRequest UploadRequest { get; set; }
    }

    internal class AddEditLocalAdheridoCommandHandler : IRequestHandler<AddEditLocalAdheridoCommand, IResult>
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork<int> _unitOfWork;
        private readonly IUploadService _uploadService;

        public AddEditLocalAdheridoCommandHandler(IMapper mapper, IUnitOfWork<int> unitOfWork, IUploadService uploadService)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _uploadService = uploadService;
        }

        public async Task<IResult> Handle(AddEditLocalAdheridoCommand request, CancellationToken cancellationToken)
        {
            //TODO fijarse cómo deberíamos de implementar la lógica para fijarse si ya hay un local agregado
            //if (await _unitOfWork.Repository<LocalAdherido>().Entities.Where(p => p.Id != request.Id).AnyAsync(cancellationToken))
            //{
            //    throw new Exception("Ya existe un local con ese nombre");
            //}

            var uploadRequest = request.UploadRequest;

            if (uploadRequest != null)
            {
                //TODO documentar el uso de la L en este caso; esto es para que aparezca con una identificación del tipo de dato que se maneja, en este caso es un local adherido, si fuese producto debería ser una ¿P=? (fijarse en addeditproductcommand.cs en blazor hero) Documentar sobre cómo se usa esta manera de guardar archivos en el readme

                uploadRequest.FileName = $"L-{request.Nombre}";
            }
            //Si es igual a cero entonces creamos uno nuevo
            if (request.Id == 0)
            {
                var local = _mapper.Map<LocalAdherido>(request);
                if (uploadRequest != null)
                {
                    local.ImageDataURL = _uploadService.Uploadsync(uploadRequest);
                }
                await _unitOfWork.Repository<LocalAdherido>().AddAsync(local);
                return await Result.SuccessAsync();
            }

            //si no, entonces modificamos el local con el id correspondiente
            else
            {
                var local = await _unitOfWork.Repository<LocalAdherido>().GetByIdAsync(request.Id);

                if (local != null)
                {
                    local.Nombre = request.Nombre ?? local.Nombre;
                    local.Descripcion = request.Descripcion ?? local.Descripcion;
                    if (uploadRequest != null)
                    {
                        local.ImageDataURL = _uploadService.Uploadsync(uploadRequest);
                    }
                    await _unitOfWork.Repository<LocalAdherido>().UpdateAsync(local);

                    return await Result.SuccessAsync();
                }
                else
                {
                    return await Result.FailAsync("Local no encontrado");
                }
            }
        }
    }
}
