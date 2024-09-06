using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace finTrack.Dto.Transaction
{
    public class TransactionResponseDto
    {
        public int TransactionID { get; set; }
        public DateTime DateTime { get; set; }
        public decimal Amount { get; set; }
        public string Action { get; set; }
        public string? FromUserID { get; set; }
        public string? ToUserID { get; set; }
    }
}