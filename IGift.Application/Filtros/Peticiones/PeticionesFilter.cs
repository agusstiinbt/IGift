using IGift.Domain.Entities;

namespace IGift.Application.Filtros.Pedidos
{
    public class PeticionesFilter : Specification<Petitions>
    {
        public PeticionesFilter(string filtroBusqueda)
        {
            /*AddInclude(a => a.Categoria);*//* Esto se usa para evitar LazyLoading y hacer un EagleLoading*/
            if (!string.IsNullOrEmpty(filtroBusqueda))
            {
                Criteria = p => (p.Descripcion != null && p.Categoria != null) && (p.Descripcion.Contains(filtroBusqueda) || p.Categoria.Contains(filtroBusqueda));
            }
        }
    }
}
