using AutoMapper;
using IGift.Application.Interfaces.Repositories;
using IGift.Application.Requests.Files;
using IGift.Shared;
using IGift.Shared.Wrapper;
using MediatR;
using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;

namespace IGift.Application.Features.LocalesAdheridos
{
    public class AddEditLocalAdheridoCommand : IRequest<Result>
    {
        /// <summary>
        /// Si se envía igual a 0(cero) significa que estamos editando un local
        /// </summary>
        public int Id { get; set; }
        [Required]
        public string Nombre { get; set; } = string.Empty;
        public bool Activo { get; set; } = true;
        public string CreatedBy { get; set; } = AppConstants.AdminEmail;
        public string Descripcion { get; set; } = string.Empty;
        public UploadRequest UploadRequest { get; set; }
    }

    internal class AddEditLocalAdheridoCommandHandler : IRequestHandler<AddEditLocalAdheridoCommand, Result>
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork<int> _unitOfWork;

        public async Task<Result> Handle(AddEditLocalAdheridoCommand request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
