namespace IGift.Application.Responses.Peticiones
{
    public class PeticionesResponse
    {
        public int Id { get; set; }
        public string Descripcion { get; set; } = string.Empty;
        public int Monto { get; set; }
        public required string Moneda { get; set; }
    }
}
