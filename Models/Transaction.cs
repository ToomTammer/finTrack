using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace finTrack.Models
{
    [Table("Transaction")]
     public class Transaction
    {
        public int TransactionID { get; set; }
        public DateTime DateTime { get; set; } = DateTime.UtcNow;
        public string UserID { get; set; }
        public decimal Amount { get; set; }
        public decimal Balance { get; set; }
        public TransactionAction Action { get; set; }
        public string? FromUserID { get; set; }
        public string? ToUserID { get; set; }
        public AppUser User { get; set; }
        public AppUser? FromUser { get; set; }
        public AppUser? ToUser { get; set; }
    }

    public enum TransactionAction
    {
        Deposit,
        Withdraw,
        Transfer
    }
}


