using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using finTrack.Dto.Transaction;

namespace finTrack.Dto.Account
{
    public class AccountDto
    {
        public String Guid { get; set; }
        public String FirstName { get; set; } = string.Empty;
        public String LastName { get; set; } = string.Empty;
        public decimal Balance { get; set; } = 0.00m;
        public DateTime CreatedAt { get; set; } = DateTime.MinValue;
        public String CreatedAT_str { get; set; } = string.Empty;
        public ICollection<TransactionDto> Transactions { get; set; } 
    }
}