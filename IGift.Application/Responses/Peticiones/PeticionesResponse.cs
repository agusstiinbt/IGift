﻿namespace IGift.Application.Responses.Pedidos
{
    public class PeticionesResponse
    {
        public string Descripcion { get; set; } = string.Empty;
        public int Monto { get; set; }
        public  string Moneda { get; set; }
    }
}
