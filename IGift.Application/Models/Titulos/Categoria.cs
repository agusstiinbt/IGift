using IGift.Domain.Contracts;
using IGift.Domain.Entities;

namespace IGift.Application.Models.Titulos
{
    public class Categoria : Entity<int>
    {
        public string Descripcion { get; set; } = string.Empty;
        public int? IdPadre { get; set; }
        public virtual ICollection<Peticiones>? Peticiones { get; set; }
    }
}
