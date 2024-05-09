
namespace IGift.Domain.Entities
{
    public class GiftCard
    {
        public int Id { get; set; }
        public int IdLocalAdherido { get; set; }
        /// <summary>
        /// Hace referencia a ApplicationUser
        /// </summary>
        public int IdUser { get; set; }
        public int Monto { get; set; }
        public virtual LocalAdherido Local { get; set; }
        //public virtual ApplicationUser User { get; set; }
    }
}
