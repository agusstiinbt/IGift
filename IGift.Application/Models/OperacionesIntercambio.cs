namespace IGift.Application.Models
{
    /// <summary>
    /// Guardar en Oracle.
    /// </summary>
    public class OperacionesIntercambio
    {
        public int Id { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime LastModifiedOn { get; set; }

        public required int IdGiftCard { get; set; }
        public required int IdSmartContract { get; set; }
        public required string IdUser1 { get; set; }
        public required string IdUser2 { get; set; }
    }
}
