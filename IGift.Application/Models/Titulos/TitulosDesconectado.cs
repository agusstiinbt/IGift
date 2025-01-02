using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IGift.Domain.Contracts;

namespace IGift.Application.Models.Titulos
{
    public class TitulosDesconectado : Entity<int>
    {
        public string Descripcion { get; set; } = string.Empty;
    }
}
