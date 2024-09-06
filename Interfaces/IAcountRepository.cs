using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using finTrack.Models;

namespace finTrack.Interfaces
{
    public interface IAcountRepository
    {
        Task<AppUser> GetUserByGuidAsync(string userGuid);
        Task<AppUser?> GetUserByUserNameAsync(string userName);
        Task<AppUser> UpdateUserAsync(AppUser appUser);

    }
}