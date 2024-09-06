using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using finTrack.Models;

namespace finTrack.Interfaces
{
    public interface ITokenService
    {
        string CreateToken(AppUser user);
        Task<string> ValidateJwtTokenAndGetUserID(string token);
    }
}