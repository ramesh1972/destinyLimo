using DestinyLimo.Common.DB;

namespace DestinyLimo.Repositories.impl
{
    public class RepositoryManager : IRepositoryManager
    {
        private DapperContext _dapperContext;
        private IRoleRepository? _roleRepository;
        private IUserRepository? _userRepository;

        public RepositoryManager(DapperContext dapperContext)
        {
            _dapperContext = dapperContext;
        }

        public IRoleRepository Role
        {
            get
            {
                _roleRepository ??= new RoleRepository(_dapperContext);
                return _roleRepository;
            }
        }

        public IUserRepository User
        {
            get
            {
                _userRepository ??= new UserRepository(_dapperContext);
                return _userRepository;
            }
        }

        //public void Save() => _dapperContext .SaveChanges();
    }
}