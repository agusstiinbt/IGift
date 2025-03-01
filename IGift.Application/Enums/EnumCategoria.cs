using System.ComponentModel;

namespace IGift.Application.Enums
{
    public enum EnumCategoria
    {
        Vehiculos,
        Inmuebles,
        Calzado,
        Supermercado,
        Tecnologia,
        [Description("Compra Internacional")]
        CompraInternacional,
        [Description("Hogar y Muebles")]
        HogarYMuebles,
        [Description("Juegos y Juguetes")]
        JuegosYJuguetes,
        Electrodomesticos,
        Servicios,
        [Description("Salud y Belleza")]
        SaludYBelleza
    }
}
