using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace finTrack.Models
{
    public class AppUser : IdentityUser
    {
        public String Guid { get; set; }
        public String FirstName { get; set; } = string.Empty;
        public String LastName { get; set; } = string.Empty;
        public decimal Balance { get; set; } = 0.00m;

        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public ICollection<Transaction> Transactions { get; set; }
    }
}