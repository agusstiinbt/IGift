using IGift.Domain.Contracts;

namespace IGift.Domain.Entities
{
    public class Contract : AuditableEntity<int>
    {
        public Contract(int idUser1, int idUser2, string createdBy, DateTime createdOn, DateTime? lastModifiedOn)
        {
            IdUser1 = idUser1;
            IdUser2 = idUser2;
            CreatedBy = createdBy;
            CreatedOn = createdOn;
            LastModifiedOn = lastModifiedOn;
        }

        public int Id { get; set; }
        public int IdUser1 { get; set; }
        public int IdUser2 { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime? LastModifiedOn { get; set; }

    }
}
