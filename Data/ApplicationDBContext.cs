using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using finTrack.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace finTrack.Controllers.Data
{
    public class ApplicationDBContext : IdentityDbContext<AppUser>
    {
        public ApplicationDBContext(DbContextOptions dbContexOptions)
        : base(dbContexOptions)
        {
            
        }

        public DbSet<Transaction> Transactions { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Transaction>()
                .HasKey(t => t.TransactionID);

            builder.Entity<Transaction>()
                .HasOne(t => t.User)
                .WithMany(u => u.Transactions)
                .HasForeignKey(t => t.UserID)
                .IsRequired(true);

            builder.Entity<Transaction>()
                .HasOne(t => t.FromUser)
                .WithMany()
                .HasForeignKey(t => t.FromUserID)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired(false);
                

            builder.Entity<Transaction>()
                .HasOne(t => t.ToUser)
                .WithMany()
                .HasForeignKey(t => t.ToUserID)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired(false);
           
            
            List<IdentityRole> roles = new List<IdentityRole>
            {
                new IdentityRole
                {
                    Name = "Amin",
                    NormalizedName = "ADMIN"
                },
                new IdentityRole
                {
                    Name = "User",
                    NormalizedName = "USER"
                }

            };
            builder.Entity<IdentityRole>().HasData(roles);
        }
    }
}