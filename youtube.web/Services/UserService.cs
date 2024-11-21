using System.Security.Claims;
using youtube.Application.Common.Interfaces;
using youtube.Domain.Entities;

namespace youtube.web.Services
{
  
    public class UserService
    {
        private readonly IApplicationUserRepository _applicationUserRepository;

        public UserService(IApplicationUserRepository applicationUserRepository)
        {
            _applicationUserRepository = applicationUserRepository;
        }

        public ApplicationUser GetLoggedInUserData(ClaimsPrincipal user)
        {
    
            var claimsIdentity = (ClaimsIdentity)user.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
            {
                throw new Exception("User ID not found in claims.");
            }


            var loggedInUser = _applicationUserRepository.Get(u => u.Id == userId);

            if (loggedInUser == null)
            {
                throw new Exception("User not found.");
            }

            return loggedInUser;
        }
    }

}
