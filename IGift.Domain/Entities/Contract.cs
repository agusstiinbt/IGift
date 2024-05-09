namespace IGift.Domain.Entities
{
    public class Contract//TODO agregar en las clases que estan en Domain la interfaz AuditableEntity de BlazorHero
    {
        public string Id { get; set; }
        public int IdUser1 { get; set; }
        public int IdUser2 { get; set; }
    }
}
