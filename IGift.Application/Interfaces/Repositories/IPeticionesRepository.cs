namespace IGift.Application.Interfaces.Repositories
{
    public interface IPeticionesRepository
    {
        Task<bool>IsBrandUsed(string id);
    }
}
