using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using finTrack.Controllers.Data;
using finTrack.Dto.Transaction;
using finTrack.Interfaces;
using finTrack.Models;
using LinqKit;
using LinqKit.Core;
using Microsoft.EntityFrameworkCore;

namespace finTrack.Repository
{
    public class TransactionRepository : ITransactionRepository
    {
        private readonly ApplicationDBContext _context;
        public TransactionRepository(ApplicationDBContext context)
        {
            _context = context;
        }
        
        public async Task<Transaction?> AddTransactionAsync(Transaction transaction)
        {
            await _context.Transactions.AddAsync(transaction);
            await _context.SaveChangesAsync();

            return transaction;

        }

        public  async Task<GroupModel> GetTransactionsAsync(TransactionDto model)
        {
            var skipNumber = (model.PageNumber - 1) * model.PageSize;

            List<Transaction> transactionsList = new List<Transaction>();

            switch (model.menuType.ToLower())
            {
                case "transfer":
                    transactionsList = await _context.Transactions
                                                .Where(c => c.Action == model.Action
                                                && c.UserID == model.UserID
                                                && c.FromUserID == model.UserID
                                                && c.ToUserID != model.UserID)
                                                .Include(t => t.FromUser)
                                                .Include(t => t.ToUser)
                                                .OrderByDescending(s => s.DateTime)
                                                .ToListAsync();
                    break;

                case "receive":
                    transactionsList = await _context.Transactions
                                                .Where(c => c.Action == model.Action
                                                && c.UserID == model.UserID
                                                && c.ToUserID == model.UserID
                                                && c.FromUserID != model.UserID)
                                                .Include(t => t.FromUser)
                                                .Include(t => t.ToUser)
                                                .OrderByDescending(s => s.DateTime)
                                                .ToListAsync();
                    break;

                default:
                    //
                    break;
            }

            List<Transaction> ManageTransactionsList = transactionsList.Skip(skipNumber).Take(model.PageSize).ToList();

            GroupModel modelGroup = new GroupModel()
            {
                transactionList = ManageTransactionsList,
                totalItemCount = transactionsList.Count(),
            };
            return modelGroup;
           
        }


    }
}