using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using finTrack.Dto.Account;
using finTrack.Models;

namespace finTrack.Dto.Transaction
{
    public class TransactionDto
    {
        public int TransactionID { get; set; }
        public DateTime DateTime { get; set; } = DateTime.Now;
        public string DateTimeStr { get; set; } = String.Empty;
        public string UserID { get; set; } 
        public string UserName { get; set; } 
        public decimal Amount { get; set; }
        public string AmountStr { get; set; }
        public decimal Balance { get; set; }
        public string BalanceStr { get; set; }
        public TransactionAction Action { get; set; }
        public string FromUserID { get; set; }
        public string FromUserName { get; set; }
        public string ToUserID { get; set; }
        public string ToUserName { get; set; }
        public int PageNumber {get; set;} = 1;
        public int PageSize {get; set;} = 10;
        public string menuType {get; set;}
        public AccountDto User { get; set; }
        public AccountDto FromUser { get; set; }
        public AccountDto ToUser { get; set; }
    }
}