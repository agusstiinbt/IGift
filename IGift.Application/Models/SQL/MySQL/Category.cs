using IGift.Domain.Contracts;
using IGift.Domain.Entities;

namespace IGift.Application.Models.SQL.MySQL
{
    public class Category : Entity<int>
    {
        public string Descripcion { get; set; } = string.Empty;
        public int? IdPadre { get; set; }
    }
}
