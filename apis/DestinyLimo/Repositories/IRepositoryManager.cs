namespace DestinyLimo.Repositories
{
    public interface IRepositoryManager
    {
        IRoleRepository Role { get; }
        IUserRepository User { get; }
        //void Save();
    }
}