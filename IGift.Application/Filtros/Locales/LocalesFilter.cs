using IGift.Domain.Entities;

namespace IGift.Application.Filtros.Locales
{
    public class LocalesFilter : Specification<LocalAdherido>
    {
        public LocalesFilter(string filtroBusqueda)
        {
            if (!string.IsNullOrEmpty(filtroBusqueda))
            {
                Criteria = l => l.Nombre != null && (l.Nombre.Contains(filtroBusqueda) || l.Descripcion.Contains(filtroBusqueda));
            }
        }
    }
}
