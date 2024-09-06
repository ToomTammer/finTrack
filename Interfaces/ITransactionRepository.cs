using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using finTrack.Dto.Transaction;
using finTrack.Models;

namespace finTrack.Interfaces
{
    public interface ITransactionRepository
    {
        Task<Transaction?> AddTransactionAsync(Transaction transaction);
        Task<GroupModel> GetTransactionsAsync(TransactionDto model);
        
    }
    
}