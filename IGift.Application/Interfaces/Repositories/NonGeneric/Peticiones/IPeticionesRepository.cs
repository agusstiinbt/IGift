namespace IGift.Application.Interfaces.Repositories.NonGeneric.Peticiones
{
    public interface IPeticionesRepository
    {
        Task<bool> IsBrandUsed(int id);
    }
}
