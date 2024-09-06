using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace finTrack.Dto.Account
{
    public class RegisterDto
    {
        [Required]
        public String FirstName { get; set; } = string.Empty;
        [Required]
        public String LastName { get; set; } = string.Empty;

        [Required]
        public string? Username { get; set; }
        [Required]
        [EmailAddress]
        public string? Email { get; set; }
        [Required]
        public string? Password { get; set; }
    }
}