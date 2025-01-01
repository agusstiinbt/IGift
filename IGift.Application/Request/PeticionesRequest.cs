namespace IGift.Application.Request
{
    /// <summary>
    /// Peticiones que se guardar en el local storage.
    /// </summary>
    public class PeticionesRequest
    {
        public int Id { get; set; }
        public string Descripcion { get; set; } = string.Empty;
        public int Monto { get; set; }
        public required string Moneda { get; set; }
    }
}
