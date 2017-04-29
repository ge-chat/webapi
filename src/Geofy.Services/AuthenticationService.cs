using System.Threading.Tasks;
using Geofy.ReadModels;
using Geofy.ReadModels.Services.User;
using Geofy.Shared.Helpers;

namespace Geofy.Services
{
    public class AuthenticationService
    {
        private readonly UserReadModelService _userReadModelService;
        private readonly CryptographicHelper _cryptographicHelper;

        public AuthenticationService(UserReadModelService userReadModelService, CryptographicHelper cryptographicHelper)
        {
            _userReadModelService = userReadModelService;
            _cryptographicHelper = cryptographicHelper;
        }

        public async Task<UserReadModel> ValidateUser(string email, string password)
        {
            var user = await _userReadModelService.GetByEmailAsync(email);
            if (user != null && user.PasswordHash == _cryptographicHelper.GetPasswordHash(password, user.PasswordSalt))
            {
                return user;
            }
            return null;
        }

        public async Task<bool> UserExists(string email)
        {
            return await _userReadModelService.GetByEmailAsync(email) != null;
        }

        public string GenerateSalt()
        {
            return _cryptographicHelper.GenerateSalt();
        }

        public string GetHash(string password, string salt)
        {
            return _cryptographicHelper.GetPasswordHash(password, salt);
        }
    }
}