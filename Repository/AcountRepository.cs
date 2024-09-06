using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using finTrack.Controllers.Data;
using finTrack.Interfaces;
using finTrack.Models;
using Microsoft.EntityFrameworkCore;

namespace finTrack.Repository
{
    public class AcountRepository : IAcountRepository
    {
        private readonly ApplicationDBContext _context;
        public AcountRepository(ApplicationDBContext context)
        {
            _context = context;
        }
        
        public async Task<AppUser?> GetUserByGuidAsync(string userId)
        {
            return await _context.Users
            .Include(u => u.Transactions)
            .FirstOrDefaultAsync(u => u.Guid == userId);
        }

        public async Task<AppUser?> GetUserByUserNameAsync(string userName)
        {
            return await _context.Users
            .Include(u => u.Transactions)
            .FirstOrDefaultAsync(u => u.UserName == userName);
        }

        public async Task<AppUser> UpdateUserAsync(AppUser appUser)
        {
            _context.Users.Update(appUser);
            await _context.SaveChangesAsync();

            return appUser;
        }
    }
}