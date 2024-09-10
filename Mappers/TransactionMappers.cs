using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using finTrack.Models;
using finTrack.Dto.Transaction;
using System.Globalization;

namespace finTrack.Mappers
{
    public static class TransactionMappers
    {
        public static TransactionDto ToTransactionDto(this Transaction model)
        {
            TimeZoneInfo venueTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
            return new TransactionDto
            {
                TransactionID = model.TransactionID,
                DateTime = model.DateTime,
                DateTimeStr = TimeZoneInfo.ConvertTimeFromUtc(model.DateTime, venueTimeZone).ToString("dd-MM-yyyy", CultureInfo.InvariantCulture.DateTimeFormat),
                UserID = model.UserID,
                AmountStr = model.Amount.ToString("F2"),
                Action = model.Action,
                FromUserID = model.FromUserID,
                ToUserID = model.ToUserID,
                UserName = $@"{model.User.FirstName} {model.User.LastName}",
                FromUserName = $@"{model.FromUser.FirstName} {model.FromUser.LastName}",
                ToUserName = $@"{model.ToUser.FirstName} {model.ToUser.LastName}",
                BalanceStr = model.Balance.ToString("F2"),
            };
        }

        public static Transaction ToTransaction(this TransactionDto model)
        {
            return new Transaction
            {
                TransactionID = model.TransactionID,
                DateTime = model.DateTime,
                UserID = model.UserID,
                Amount = model.Amount,
                Balance = model.Balance,
                Action = model.Action,
                FromUserID = model.FromUserID,
                ToUserID = model.ToUserID,
                User = model.User.ToAccount(),
                FromUser = model.FromUser.ToAccount(),
                ToUser = model.ToUser.ToAccount(),

            };
        }

        public static Transaction ToTransactionFromTransactionRequestDto(this TransactionRequestDto request)
        {
            return new Transaction
            {
                Amount = request.Amount,
                Action = Enum.Parse<TransactionAction>(request.Action),
                UserID = request.UserID,
                FromUserID = request?.FromUserID?? null,
                ToUserID = request?.ToUserID?? null,
                Balance = request.Balance

            };
        }
    }
}