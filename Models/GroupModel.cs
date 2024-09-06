using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace finTrack.Models
{
    public class GroupModel
    {
        public List<Transaction> transactionList { get; set; }
        public int totalItemCount { get; set; }
    }
}