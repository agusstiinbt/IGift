using IGift.Domain.Contracts;

namespace IGift.Application.Models
{
    public class Categoria : Entity<int>
    {
        public string Descripcion { get; set; } = string.Empty;
        public int IdPadre { get; set; }
    }
}
