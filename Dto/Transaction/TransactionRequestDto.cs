using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace finTrack.Dto.Transaction
{
    public class TransactionRequestDto
    {
        [Required]
        public decimal Amount { get; set; }
        [Required]
        public string Action { get; set; }
        public string? FromUserID { get; set; } = "fyi";
        public string? ToUserID { get; set; } = "fyi";
        public string? ToUserName { get; set; } = string.Empty;
        public string? UserID { get; set; }
        public decimal Balance { get; set; }
        
    }
}