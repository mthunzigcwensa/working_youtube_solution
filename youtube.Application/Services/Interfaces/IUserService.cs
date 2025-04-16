using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using youtube.Domain.Entities;

namespace youtube.Application.Services.Interfaces
{
    public interface IUserService
    {
        ApplicationUser GetLoggedInUserData(ClaimsPrincipal user);
    }

}
