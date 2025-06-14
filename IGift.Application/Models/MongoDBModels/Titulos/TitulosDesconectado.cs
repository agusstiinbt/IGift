using IGift.Domain.Contracts;

namespace IGift.Application.Models.MongoDBModels.Titulos
{
    public class TitulosDesconectado : Entity<int>
    {
        public string Descripcion { get; set; } = string.Empty;
    }
}
