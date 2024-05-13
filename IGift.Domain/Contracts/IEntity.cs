namespace IGift.Domain.Contracts
{
    public interface IEntity<TId> : IEntity//TODO implementar las clases de Domain con esto
    {
        public TId Id { get; set; }
    }

    public interface IEntity
    {
    }
}
