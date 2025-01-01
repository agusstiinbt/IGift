using System.ComponentModel;

namespace IGift.Application.Enums
{
    public enum EnumCategoria
    {
        Vehiculos,
        Imuebles,
        Supermercado,
        Tecnologia,
        [Description("Compra Internacional")]
        CompraInternacional,
        [Description("Hogar y Muebles")]
        HogarYMuebles,
        Electrodomesticos,
        Servicios,
        [Description("Salud y Belleza")]
        SaludYBelleza
    }
}
