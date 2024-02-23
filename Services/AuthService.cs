using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
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
            var model = await _context.AppUsers
                            .Include(u => u.AppUserRoles)
                            .ThenInclude(ur => ur.Role)
                            .FirstOrDefaultAsync(u =>
                                u.Username == username);

            if (model == null)
            {
                return null;
            }

            var roles = model.AppUserRoles.Select(r => r.Role.Name).ToList();

            var viewModel = new UserViewModel { Username = model.Username, AppUserId = model.AppUserId };

            viewModel.Roles = roles.ToArray();

            return viewModel;
        }

        public async Task<Role> GetRoleByNameAsync(string roleName)
        {
            return await _context.Roles.FirstAsync(r => r.Name == roleName);
        }

        public async Task<IEnumerable<string>> GetRolesAsync()
        {
            return await _context.Roles.Select(r => r.Name).ToListAsync();
        }

        public async Task<TokenViewModel> LoginAsync(LoginViewModel viewModel)
        {
            var user = await _context.AppUsers.FirstAsync(u => u.Username == viewModel.Username);

            var passwordMatches = VerifyPasswordHash(viewModel.Password, user.PasswordHash, user.PasswordSalt);

            if (!passwordMatches)
            {
                throw new Exception("Login failed");
            }

            //  create claims
            var userViewModel = await GetByUsernameAsync(viewModel.Username);
            
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userViewModel.Username),

            };

            foreach (var role in userViewModel.Roles)
            {
                claims.Add(new Claim("Roles", role));
            }


            //  process the secret key
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(appSecret));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            //  prepare to produce the signature
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(1),
                SigningCredentials = credentials
            };

            var tokenHandler = new JwtSecurityTokenHandler();

            //  produce the token
            var token = tokenHandler.CreateToken(tokenDescriptor);

            //  return
            return new TokenViewModel { Token = tokenHandler.WriteToken(token) };
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
