using IGift.Domain.Contracts;

namespace IGift.Domain.Entities
{
    public class Peticiones : AuditableEntity<int>
    {
        /// <summary>
        /// Id del usuario creador
        /// </summary>
        public string IdUser { get; set; } = string.Empty;
        /// <summary>
        /// Descripcion de la peticion, esto queda a libertad del cliente
        /// </summary>
        public string Descripcion { get; set; } = string.Empty;

        public required string Categoria { get; set; } = string.Empty;
        public int Monto { get; set; }
        public required string Moneda { get; set; } = string.Empty;
        /// <summary>
        /// Esta propiedad indica si se ha comprado o no la peticion
        /// </summary>
        public bool Finalizado { get; set; } = false;

        //public virtual Categoria Categoria { get; set; }
    }
}