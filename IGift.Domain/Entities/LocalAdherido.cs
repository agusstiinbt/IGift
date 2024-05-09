using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IGift.Domain.Entities
{
    public class LocalAdherido
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool Activo { get; set; }
    }

    public class InfoLocalesAdheridos
    {
        public int Id { get; set; }
        public int IdLocalAdherido { get; set; }
        public virtual required LocalAdherido LocalAdherido { get; set; }

        public string Direccion { get; set; } = string.Empty;
    }
}
