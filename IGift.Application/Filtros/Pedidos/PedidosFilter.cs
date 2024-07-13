namespace IGift.Application.Filtros.Pedidos
{
    public class PedidosFilter : Specification<Domain.Entities.Pedidos>
    {
        public PedidosFilter(string filtroBusqueda)
        {

            //Includes.Add(a => a.Brands); Esto se usa para filtrar no solamente por el nombre (descripcion) sino también por la marca
            if (!string.IsNullOrEmpty(filtroBusqueda))
            {
                Criteria = p => p.Descripcion != null && (p.Descripcion.Contains(filtroBusqueda) || p.Descripcion.Contains(filtroBusqueda));
            }
            else
            {
                Criteria = p => p.Descripcion != null;
            }
        }
    }
}
