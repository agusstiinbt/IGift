using IGift.Domain.Entities;

namespace IGift.Application.Filtros.Pedidos
{
    public class PeticionesFilter : Specification<Peticiones>
    {
        public PeticionesFilter(string filtroBusqueda)
        {

          /*  AddInclude(a => a.Categoria);*//* Esto se usa para evitar LazyLoading y hacer un EagleLoading*/
            if (!string.IsNullOrEmpty(filtroBusqueda))
            {
                Criterio = p => p.Descripcion != null && p.Descripcion.Contains(filtroBusqueda);
                
            }
            else
            {
                Criterio = p => p.Descripcion != null;
            }
        }
    }
}
