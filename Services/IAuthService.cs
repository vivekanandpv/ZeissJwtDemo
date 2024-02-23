using ZeissJwtDemo.Models;
using ZeissJwtDemo.ViewModels;

namespace ZeissJwtDemo.Services
{
    public interface IAuthService
    {
        Task RegisterAsync(UserRegisterViewModel viewModel);
        Task<UserViewModel> GetByUsernameAsync(string username);
        Task<IEnumerable<string>> GetRolesAsync();
        Task<Role> GetRoleByNameAsync(string roleName);
        Task<TokenViewModel> LoginAsync(LoginViewModel viewModel);
    }
}
