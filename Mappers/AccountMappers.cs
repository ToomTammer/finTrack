using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using finTrack.Models;
using finTrack.Dto.Account;
using System.Globalization;

namespace finTrack.Mappers
{
    public static class AccountMappers
    {
        public static AccountDto ToAccountDto(this AppUser model)
        {
            TimeZoneInfo venueTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
            return new AccountDto
            {
                Guid  = model.Guid,
                FirstName  = model.FirstName,
                LastName  = model.LastName,
                Balance  = model.Balance,
                CreatedAt  = model.CreatedAt,
                CreatedAT_str  = TimeZoneInfo.ConvertTimeFromUtc(model.CreatedAt, venueTimeZone).ToString("dd-MM-yyyy", CultureInfo.InvariantCulture.DateTimeFormat),
                Transactions  = model.Transactions.Select(c => c.ToTransactionDto()).ToList(),
            };
        }

        public static AppUser ToAccount(this AccountDto model)
        {
            return new AppUser
            {
                Guid  = model.Guid,
                FirstName  = model.FirstName,
                LastName  = model.LastName,
                Balance  = model.Balance,
                CreatedAt  = model.CreatedAt,
                Transactions  = model.Transactions.Select(c => c.ToTransaction()).ToList(),
            };
        }
    }
}