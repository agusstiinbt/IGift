using IGift.Domain.Contracts;

namespace IGift.Application.Models
{
    /// <summary>
    /// Guardar en Oracle.
    /// </summary>
    public class OperacionesIntercambio : Entity<int>
    {
        public DateTime CreatedOn { get; set; }
        public required int IdGiftCard { get; set; }
        public required int IdSmartContract { get; set; }
        public required string IdUser1 { get; set; }
        public required string IdUser2 { get; set; }
    }
}
