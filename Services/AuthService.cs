using ZeissJwtDemo.Context;
using ZeissJwtDemo.Models;
using ZeissJwtDemo.ViewModels;

namespace ZeissJwtDemo.Services
{
    public class AuthService : IAuthService
    {
        private readonly AuthProjectContext _context;
        private readonly string appSecret;

        public AuthService(AuthProjectContext context, IConfiguration configuration)
        {
            _context = context;
            appSecret = configuration.GetValue<string>("ServerSecret");
        }

        public async Task<UserViewModel> GetByUsernameAsync(string username)
        {
            throw new NotImplementedException();
        }

        public async Task<Role> GetRoleByNameAsync(string roleName)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<string>> GetRolesAsync()
        {
            throw new NotImplementedException();
        }

        public async Task<TokenViewModel> LoginAsync(LoginViewModel viewModel)
        {
            throw new NotImplementedException();
        }

        public async Task RegisterAsync(UserRegisterViewModel viewModel)
        {
            var user = new AppUser { Username = viewModel.Username };
            byte[] passwordHash, passwordSalt;

            CreatePasswordHash(viewModel.Password, out passwordHash, out passwordSalt);

            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;

            await _context.AddAsync(user);
            await _context.SaveChangesAsync();

            foreach (var role in viewModel.Roles)
            {
                var modelRole = await GetRoleByNameAsync(role);

                if (modelRole != null)
                {
                    var userRoleModel = new AppUserRole
                    {
                        AppUser = user,
                        Role = modelRole,
                        AppUserId = user.AppUserId,
                        RoleId = modelRole.RoleId
                    };

                    //  add user-roles
                    await _context.AddAsync(userRoleModel);
                }
            }

            await _context.SaveChangesAsync();
        }

        private void CreatePasswordHash(string rawPassword, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac = new System.Security.Cryptography.HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(rawPassword));
            }
        }

        private bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using (var hmac = new System.Security.Cryptography.HMACSHA512(passwordSalt))
            {
                var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));

                for (int i = 0; i < computedHash.Length; i++)
                {
                    if (computedHash[i] != passwordHash[i])
                    {
                        return false;
                    }
                }
            }

            return true;
        }
    }
}
